# Конвенции кода

- Эндпоинт-на-файл. `Group<{Area}Group>()` на фичу. Авторизация — `Policies(PolicyNames.*)` из общих констант.
- **Эндпоинт тонкий**: route-резолв + вызов сервиса + формирование Response. Никакого `AppDbContext`, чтения claims или `.Select`-проекций внутри эндпоинта.
- Модификаторы: `public override async Task ...` (не `async override`).
- `CancellationToken ct` пробрасывается в каждый async-вызов (EF, `SaveChangesAsync`, сервисы).
- Разделение `Models/DTOs`, `Models/Requests`, `Models/Responses`. Списки — через `BaseListResponse<T>`.
- Валидация — `BaseValidator<T>` (FluentValidation, авто-подхват FastEndpoints); файлы — `BaseImageFileValidator` / `BaseAudioFileValidator`.
- Только серверные LINQ-проекции (`EF.Functions.ILike`, `.Select` транслируемый в SQL). Без `.ToList().Where()` и client-eval.
- Async-методы сервисов — с суффиксом `Async`.
- Условная композиция запроса — через `PipeExtensions` (`PipeAs`) там, где есть ветвление; не пихать ради церемонии.
