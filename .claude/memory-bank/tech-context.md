# Tech Context

- **.NET 8 / C#.** FastEndpoints, EF Core 8 / Npgsql (PostgreSQL).
- FluentValidation, Serilog, Otp.NET + QRCoder (2FA TOTP), BCrypt.Net-Next (хеш паролей).
- Тесты: xUnit + Microsoft.EntityFrameworkCore.InMemory.
- **Docker**: multi-stage `Dockerfile` + `docker-compose.yml` (api + postgres). Миграции применяются на старте (`Database.Migrate()` в `Program.cs`).
- **Секреты — через env / user-secrets, НЕ в репо.** Ключи: `ConnectionStrings:Audio_player`, `AuthOptions:Key` (≥32 символа), `ImageStore:FilesPath`, `AudioStore:FilesPath`, `Cors:AllowedOrigins`.
- Запуск: `docker compose up --build` → API `:8080`, Swagger `/swagger`.
- Локальный dev-SDK — .NET 10 preview, но таргет проектов — `net8.0` (использовать образы 8.0).
