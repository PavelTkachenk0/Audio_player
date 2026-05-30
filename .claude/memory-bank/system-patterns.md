# System Patterns

- **Слои**: тонкий `Endpoint` → `Service` (логика + `DbContext`) → `Mapper` (проекция Entity→DTO).
- **Мапперы**: `IQueryable`-extension `EntityToDto` — единый источник серверной проекции. Вторичные — `EntityToShortDto` / `EntityToByAlbumIdDto`; поиск — `ToSearchDto`.
- **Без репозиториев** поверх EF (осознанное решение — EF уже UoW + Repository).
- **Auth**: JWT (access 15 мин + refresh 7 дней с ротацией). 2FA двухшаговая: `/login` при включённой 2FA отдаёт `RequiresTwoFactor` + pre-auth токен, вход завершается на `/verify-2fa`. Отзыв access — по `jti` в `JwtBearerEvents.OnTokenValidated` (после проверки подписи). `AccessTokens`-таблица хранится сознательно (stateful-отзыв).
- **userId-резолв**: `AppDbContext.GetCurrentUserProfileIdAsync(ClaimsPrincipal, ct)` (extension в `Audio_player.Extensions`) — единый способ, заменил ~27 inline-копий.
- **DAL**: `BaseEntity<TKey>` (long для крупных сущностей, short для справочников); явные M2M join-сущности (`UserSongs`, `ArtistAlbum`…); `IEntityTypeConfiguration` рядом с моделью.
- **Ошибки**: FastEndpoints возвращает 400 на валидацию (`config.Errors.StatusCode = 400`); not-found → `SendNotFoundAsync`.
