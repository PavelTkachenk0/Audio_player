# Архитектура

Vertical-slice (FastEndpoints) + сервисный слой + доменные мапперы. Проекты: `Audio_player` (API) и `Audio_player.DAL` (данные).

## Поток запроса

```
Endpoint (тонкий)  →  Service (бизнес-логика + DbContext)  →  Mapper (проекция в DTO)
```

## Правила

- **Сервисы** (`Services/{Area}Service.cs`): primary constructor (`AppDbContext` [+ `FileService`, если файлы]). GET-методы принимают `ClaimsPrincipal` и резолвят пользователя через `_appDbContext.GetCurrentUserProfileIdAsync(user, ct)`. Create возвращает `Task`; Update/Delete — `Task<bool>` (`false` = не найдено) либо результат-enum для сложных исходов (см. `DeleteUserResult`). `ct` в каждом async-вызове.
- **Мапперы** (`Mappers/{Entity}Mapper.cs`): `public static IQueryable<Dto> EntityToDto(this IQueryable<Entity> q, long userId)`. Единый источник серверной проекции. Вторичные проекции — отдельным именем (`EntityToShortDto`, `EntityToByAlbumIdDto`); поиск — `ToSearchDto`.
- **Репозитории поверх EF НЕ вводить.** `DbContext` = Unit of Work, `DbSet<T>` = Repository. Обёртки прячут `IQueryable` и ломают серверные проекции / `Include`.
- **DAL**: каждая модель наследует `BaseEntity<TKey>`; её `IEntityTypeConfiguration` лежит в том же файле; авторегистрация через `ApplyConfigurationsFromAssembly`. M2M — через явные join-сущности.
- **DI**: все доменные сервисы регистрируются `AddScoped` в `Program.cs`.
- **Auth**: токены/2FA инкапсулированы в `AuthService` + `GenerateTokenService`; отзыв access по `jti` — в `JwtBearerEvents.OnTokenValidated`.
