# Audio Player — дизайн микросервисной архитектуры

> Статус: **дизайн** (реализация по этапам, strangler). Цель — учебная, но «по-взрослому»: брокер сообщений, БД-на-сервис, eventual consistency, observability.

---

## 1. Цели и принципы

- **Bounded contexts по данным, а не по коду.** Каждый сервис владеет своими таблицами; чужие данные — только через API или события.
- **Никаких распределённых join'ов.** Library не ходит в Catalog-БД; держит собственную read-модель, обновляемую событиями.
- **Слабая связанность через брокер.** Синхронные цепочки вызовов минимизируем — иначе получим «распределённый монолит».
- **Strangler-миграция.** На каждом шаге система остаётся рабочей; монолит «обрастается» сервисами и постепенно опустошается.
- **Внутренний паттерн сервисов сохраняем** — тонкие эндпоинты + сервисы + мапперы (как в монолите).

---

## 2. Целевая архитектура

```
                              ┌──────────────────┐
        клиент  ───────────►  │  API Gateway     │  YARP
                              │  (маршруты + JWT) │
                              └───┬───┬───┬───┬───┘
              ┌───────────────────┘   │   │   └───────────────────┐
              ▼                       ▼   ▼                       ▼
      ┌──────────────┐      ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
      │  Identity    │      │   Catalog    │  │   Library    │  │  Streaming   │
      │  users/auth  │      │  albums      │  │  favorites   │  │  SignalR hub │
      │  2FA/tokens  │      │  artists     │  │  recommend.  │  │  audio       │
      │  RS256 issuer│      │  tracks…     │  │  read-model  │  │              │
      └──────┬───────┘      └──────┬───────┘  └──────┬───────┘  └──────┬───────┘
         identity-db          catalog-db         library-db         media-volume
                                    │                  ▲
                                    │   события         │  подписка
                                    └────►  RabbitMQ  ───┘
                                          (MassTransit)
```

JWT подписывает только **Identity** (приватный ключ RS256). Остальные валидируют его **публичным** ключом локально — без обращения в Identity на каждый запрос.

---

## 3. Сервисы

| Сервис | Ответственность | БД | Наружу (через Gateway) |
|--------|-----------------|-----|------------------------|
| **Gateway** | маршрутизация, терминирование JWT-проверки (опц.), rate-limit | — | `/*` |
| **Identity** | регистрация, логин, 2FA, refresh-ротация, выдача RS256-JWT | `identity-db` | `/auth/*`, `/users/*` |
| **Catalog** | CRUD каталога, поиск; **публикует** события об изменениях | `catalog-db` | `/albums`, `/artists`, `/tracks`, `/genres`, `/playlists`, `/search` |
| **Library** | избранное, рекомендации, история; **потребляет** события Catalog в read-модель | `library-db` | `/favorites/*`, `/recommendations/*` |
| **Streaming** | SignalR-стриминг аудио, счётчик прослушиваний | media volume | `/audioHub` |

Внутренняя структура каждого — прежняя: `Endpoints / Services / Mappers / Validators`, плюс `Consumers/` и `Contracts/` у тех, кто на шине.

---

## 4. Декомпозиция данных и read-модель

Главная сложность — `Library` сейчас джойнит `UserSongs → Song → Album/Artists` и проецирует в `TrackDTO` с названиями/обложками. После разреза:

- **Library владеет** только связями «пользователь ↔ id сущности»: `UserSongs`, `UserAlbums`, `UserArtists`, `UserPlaylists` (по сути — id + userId).
- **Названия/обложки/исполнители** Library **дублирует** в собственной read-модели (`CatalogTrackView`, `CatalogAlbumView`…), обновляемой событиями Catalog.

```
Catalog:  Track{ Id, Name, AlbumId, Duration, ... }   ──TrackUpserted──►  Library: CatalogTrackView{ Id, Name, AlbumCover, Artists[] }
```

При запросе «моё избранное» Library читает **только свою БД** (join локальной read-модели и UserSongs) — никаких походов в Catalog. Цена — eventual consistency (название обновится через секунды после события).

---

## 5. Event catalog

Контракты событий — отдельная общая библиотека `BuildingBlocks.Contracts` (только DTO событий, версионируемые).

| Событие | Издатель | Потребители | Назначение |
|---------|----------|-------------|------------|
| `TrackUpserted` | Catalog | Library | создать/обновить `CatalogTrackView` |
| `TrackDeleted` | Catalog | Library | удалить view + осиротевшие favorites |
| `AlbumUpserted` / `AlbumDeleted` | Catalog | Library, Streaming | обложки/метаданные |
| `ArtistUpserted` | Catalog | Library | имена исполнителей |
| `TrackListened` | Streaming | Library, Catalog | история + `ListeningCount` |
| `UserDeleted` | Identity | Library, Catalog | каскадная чистка пользовательских данных |

Топология MassTransit: publish → exchange по типу события → отдельная очередь на каждого консьюмера (fan-out, независимые подписчики).

---

## 6. Паттерны надёжности

- **Transactional Outbox** (Catalog): запись в БД + событие в outbox-таблицу в одной транзакции; отдельный процесс публикует из outbox. Гарантия «не потеряем событие при падении после commit». MassTransit умеет outbox из коробки (EF).
- **Idempotent consumers** (Library): обработка `TrackUpserted` идемпотентна (upsert по Id) — повтор доставки безопасен (at-least-once).
- **Retry + DLQ**: транзиентные ошибки — экспоненциальный retry; неустранимые — в dead-letter-queue для разбора.
- **Saga (опц., Этап 3+)**: многошаговые операции (если появятся) — через MassTransit state machine вместо распределённых транзакций.
- **Versioning событий**: новые поля — опциональны; ломающие изменения — новый тип/версия.

---

## 7. Аутентификация между сервисами

- Сейчас HS256 (симметричный ключ) — для микросервисов плохо (секрет нужен всем).
- Переходим на **RS256**: Identity держит приватный ключ и подписывает; остальные валидируют публичным (раздаём через JWKS-эндпоинт Identity или конфигом).
- Gateway может делать предварительную проверку, но каждый сервис всё равно валидирует сам (defense in depth).
- `userId` — в claim; межсервисные вызовы (если нужны) — отдельный machine-to-machine токен (client credentials) или внутренняя сеть + mTLS.

---

## 8. Инфраструктура

- **Брокер**: RabbitMQ + **MassTransit** (.NET-абстракция: consumers, outbox, retry, DLQ, тестируемость).
- **БД**: PostgreSQL, отдельный инстанс/схема на сервис (`identity-db`, `catalog-db`, `library-db`).
- **Gateway**: YARP (reverse proxy на .NET).
- **Оркестрация**: `docker-compose` (dev) → опц. k8s. Сервисы + RabbitMQ + N баз + (Этап 4) observability-стек.
- **Observability (Этап 4)**: OpenTelemetry (трейсинг сквозь HTTP + шину), Serilog → централизованные логи, health-checks на каждом сервисе.

---

## 9. Структура решения (целевая)

```
src/
  Gateway/                        — YARP
  Services/
    Identity/        Identity.Api,  Identity.Domain,  identity-db
    Catalog/         Catalog.Api,   Catalog.Domain,   catalog-db   (+ Outbox, Publishers)
    Library/         Library.Api,   Library.Domain,   library-db   (+ Consumers, ReadModel)
    Streaming/       Streaming.Api
  BuildingBlocks/
    Contracts/                     — события (общие DTO)
    Messaging/                     — конфигурация MassTransit/RabbitMQ
    Auth/                          — общая JWT RS256-валидация
tests/                            — по сервису
docker-compose.yml                — всё вместе
```

Текущий монолит `Audio_player` постепенно «раздаётся» в эти сервисы и в конце удаляется.

---

## 10. План миграции (strangler)

| Этап | Что | Результат |
|------|-----|-----------|
| **0** | Вынести общие BuildingBlocks; поднять **Gateway (YARP)** перед монолитом; RS256-токены | снаружи без изменений, точка входа — Gateway |
| **1** | **Identity**: свой сервис + `identity-db`, выдача RS256-JWT, JWKS; монолит валидирует публичным ключом | auth отделён |
| **2** | **Streaming**: вынести SignalR-хаб в отдельный сервис | особый профиль нагрузки изолирован |
| **3** | **Catalog ↔ Library**: разрезать БД, RabbitMQ + MassTransit, outbox в Catalog, read-модель в Library | ⭐ брокер в деле, eventual consistency |
| **4** | **Observability + оркестрация**: OpenTelemetry-трейсинг, health-checks, причёска docker-compose/k8s | production-grade наблюдаемость |

**Принцип:** каждый этап — рабочая система. Не «большой взрыв».

---

## 11. Риски и решения

| Риск | Решение |
|------|---------|
| Распределённый монолит (синхронные цепочки) | максимум async через события; sync — только Gateway→сервис |
| Потеря событий | transactional outbox |
| Двойная доставка | идемпотентные консьюмеры (upsert) |
| Рассогласование read-модели | события + (опц.) периодическая сверка/реконсиляция |
| Сложность локального запуска | один `docker-compose up` поднимает весь ландшафт |
| Отладка сквозь сервисы | OpenTelemetry distributed tracing (Этап 4) |

---

## Следующие шаги

1. Согласовать этот дизайн.
2. **Этап 0**: завести структуру решения `src/`, `BuildingBlocks`, Gateway, перейти на RS256.
3. Дальше по таблице, по одному этапу, с тестами и рабочим `docker-compose` на каждом шаге.
