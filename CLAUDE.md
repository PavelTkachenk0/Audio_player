# Audio Player — гайд для ИИ-ассистента

Backend музыкального стримингового сервиса на C# / .NET 8 (FastEndpoints + EF Core / PostgreSQL).
Двухпроектная архитектура: API + DAL, плюс тестовый проект.

## Команды

```bash
dotnet build Audio_player/Audio_player.sln                 # сборка
dotnet test  Audio_player.Tests/Audio_player.Tests.csproj  # тесты
docker compose up --build                                  # запуск (API :8080, Swagger /swagger)
# миграция:
dotnet ef migrations add <Name> --project Audio_player.DAL --startup-project Audio_player
```

## Структура

| Путь | Что внутри |
|------|-----------|
| `Audio_player/` | API: `Endpoints/`, `Services/`, `Mappers/`, `Validators/`, `Hubs/`, `Extensions/` |
| `Audio_player.DAL/` | модели (`BaseEntity<TKey>`), `IEntityTypeConfiguration` рядом с моделью, миграции, `AppDbContext` |
| `Audio_player.Tests/` | xUnit (+ EF Core InMemory) |

## Контекст и правила

Перед изменениями ознакомься:

- @.claude/rules/architecture.md — слои, паттерн «сервис + маппер»
- @.claude/rules/conventions.md — стиль эндпоинтов и кода
- @.claude/rules/testing.md — как пишутся тесты
- @.claude/memory-bank/project-brief.md — что за проект и зачем
- @.claude/memory-bank/tech-context.md — стек и запуск
- @.claude/memory-bank/system-patterns.md — архитектурные решения
- @.claude/memory-bank/progress.md — что сделано / в работе / чего НЕ делать

## Ключевое (TL;DR)

1. **Эндпоинты тонкие**: инжектят только свой `*Service`, резолвят route-значения, зовут сервис, формируют Response. Никакого `AppDbContext`, claims или проекций в эндпоинте.
2. **Логика — в `Services/`**, **проекции Entity→DTO — в `Mappers/`** как `IQueryable`-extension `EntityToDto` (единый источник; не inline).
3. **Репозитории поверх EF НЕ вводить** — `DbContext` уже Unit of Work + Repository.
4. **`CancellationToken` пробрасывать везде**; серверные LINQ-проекции (без client-eval).
5. **Тесты** — xUnit `[Theory]`; db-тесты разнесены на `Tests` / `TestCases` / `TestData`.
6. `main` под branch-protection (require PR) — изменения через ветку + PR.
