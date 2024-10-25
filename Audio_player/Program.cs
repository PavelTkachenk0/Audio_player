using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Helpers;
using Audio_player.Hubs;
using Audio_player.Middlewares;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

    builder.Host
        .UseSerilog((context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
        );

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseStaticFiles();

    app.UseFastEndpoints(config =>
    {
        config.Endpoints.RoutePrefix = "api";
        config.Errors.StatusCode = 418;
    })
    .UseSwaggerGen();

    app.UseCors("CorsPolicy");

    app.UseMiddleware<AccessTokenValidationMiddleware>();

    app.UseRouting();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapHub<AudioHub>("/audioHub");

    app.MapDefaultControllerRoute();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application run failure");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
{
    services.SwaggerDocument(swagger =>
    {
    });

    services.AddDbContext<AppDbContext>(opt =>
    {
        opt.UseNpgsql(configuration.GetConnectionString(nameof(Audio_player)));
    });

    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true));
    });

    services.AddFastEndpoints(o =>
     {
         o.SourceGeneratorDiscoveredTypes.AddRange(Audio_player.DiscoveredTypes.All);
     });

    ConfigureAuth(services, configuration);

    services.AddControllers();

    services.AddSignalR();

    services.AddScoped<GenerateTokenService>();

    services.AddOptions<FileStoreOptions>().BindConfiguration("FileStore");

    services.AddOptions<AuthOptions>().BindConfiguration(nameof(AuthOptions));
}

static void ConfigureAuth(IServiceCollection services, IConfiguration configuration)
{
    var authOptions = new AuthOptions();

    configuration.GetSection(nameof(AuthOptions)).Bind(authOptions);

    services.AddSingleton(authOptions);

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = authOptions.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
                RoleClaimType = ClaimTypes.Role
            };
        });

    services.AddAuthorization(auth =>
    {
        auth.FallbackPolicy = auth.DefaultPolicy;

        auth.AddPolicy(PolicyNames.HasAdminRole, p =>
        {
            p.RequireAssertion(c => c.Resource is HttpContext httpContext
                ? c.User.IsInRole(Roles.Admin) : false);
        });

        auth.AddPolicy(PolicyNames.HasUserRole, p =>
        {
            p.RequireAssertion(c => c.Resource is HttpContext httpContext
                ? c.User.IsInRole(Roles.User) : false);
        });

        auth.AddPolicy(PolicyNames.HasAdminOrUserRole, p =>
        {
            p.RequireAssertion(c => c.Resource is HttpContext httpContext
                ? (c.User.IsInRole(Roles.Admin) || c.User.IsInRole(Roles.User))
                : false);
        });
    });
}