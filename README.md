# 🎵 Audio Player

Backend музыкального стримингового сервиса на C# / .NET 8 (FastEndpoints + EF Core / PostgreSQL). REST API с JWT + 2FA и потоковой передачей аудио через SignalR.

## Запуск

```bash
docker compose up --build
```

API — `http://localhost:8080`, Swagger — `/swagger`. Секреты в `docker-compose.yml` только для разработки.

Локально: подними PostgreSQL, задай `ConnectionStrings:Audio_player` и `AuthOptions:Key` (через user-secrets), затем `dotnet run --project Audio_player`. Тесты — `dotnet test`.
