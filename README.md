<div align="center">

# 🎵 Audio Player

**Backend музыкального стримингового сервиса на C# / .NET 8**

REST API с JWT + 2FA, потоковой передачей аудио через SignalR и чистой сервисной архитектурой.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-green)
![Tests](https://img.shields.io/badge/tests-23%20passing-brightgreen)
![Docker](https://img.shields.io/badge/docker-ready-2496ED?logo=docker&logoColor=white)

</div>

---

## ✨ Возможности

- 🎧 **Каталог** — треки, альбомы, исполнители, жанры, плейлисты (CRUD + поиск по названию)
- ❤️ **Избранное и рекомендации** — персонально по пользователю
- 🔐 **Аутентификация** — JWT (access + refresh с ротацией) и двухшаговая 2FA (TOTP + QR-код)
- 📡 **Стриминг** — потоковая передача MP3 через SignalR
- 🔎 **Сквозной поиск** по всему каталогу

## 🏗️ Архитектура и стек

| Слой | Технологии |
|------|-----------|
| API | **FastEndpoints** (REPR), тонкие эндпоинты |
| Логика | **сервисный слой** + доменные мапперы (`EntityToDto`) — без репозиториев поверх EF |
| Данные | **EF Core** / **PostgreSQL**, `IEntityTypeConfiguration` рядом с моделями |
| Прочее | FluentValidation · Serilog · xUnit |

**Безопасность:** BCrypt-хеширование паролей · CORS-allowlist · серверный отзыв токенов · секреты через переменные окружения.

## 🚀 Запуск

```bash
docker compose up --build
```

API — `http://localhost:8080`, Swagger UI — `/swagger`. PostgreSQL и миграции поднимаются автоматически.

<details>
<summary>Локально, без Docker</summary>

1. Подними PostgreSQL.
2. Задай `ConnectionStrings:Audio_player` и `AuthOptions:Key` через `dotnet user-secrets`.
3. `dotnet run --project Audio_player`

</details>

## 🧪 Тесты

```bash
dotnet test
```

## 📁 Структура

```
Audio_player/        API — Endpoints, Services, Mappers, Validators, Hubs
Audio_player.DAL/    модели, конфигурации, миграции, AppDbContext
Audio_player.Tests/  xUnit (unit + EF Core InMemory)
```

## 📄 Лицензия

[MIT](LICENSE)
