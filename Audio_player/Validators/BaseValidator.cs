using Audio_player.DAL;
using FastEndpoints;

namespace Audio_player.Validators;

public abstract class BaseValidator<TRequest> : Validator<TRequest>
    where TRequest : class
{
    protected AppDbContext DbContext => Resolve<AppDbContext>();

    protected IHttpContextAccessor HttpContextAccessor => Resolve<IHttpContextAccessor>();

    protected IConfiguration Configuration => Resolve<IConfiguration>();
}
