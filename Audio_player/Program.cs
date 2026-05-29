using Audio_player.AppSettingsOptions;
using Audio_player.Constants;
using Audio_player.DAL;
using Audio_player.Helpers;
using Audio_player.Hubs;
using Audio_player.Services;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
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

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseStaticFiles();

    app.UseFastEndpoints(config =>
    {
        config.Endpoints.RoutePrefix = "api";
        config.Errors.StatusCode = 400;
    })
    .UseSwaggerGen();

    app.UseCors("CorsPolicy");

    app.UseRouting();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapHub<AudioHub>("/audioHub").RequireAuthorization();

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
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? Array.Empty<string>();

        options.AddPolicy("CorsPolicy", builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins(allowedOrigins));
    });

    services.AddFastEndpoints(o =>
     {
         o.SourceGeneratorDiscoveredTypes.AddRange(Audio_player.DiscoveredTypes.All);
     });

    ConfigureAuth(services, configuration);

    services.AddControllers();

    services.AddSignalR();

    services.AddScoped<GenerateTokenService>();

    services.AddScoped<FileService>();

    services.AddScoped<AlbumService>();
    services.AddScoped<TrackService>();
    services.AddScoped<ArtistService>();
    services.AddScoped<GenreService>();
    services.AddScoped<GenrePlaylistService>();
    services.AddScoped<UserService>();
    services.AddScoped<RecommendationService>();
    services.AddScoped<FavoriteService>();
    services.AddScoped<SearchService>();

    services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

    ConfigureAppSettings(services, configuration);
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

            options.Events = new JwtBearerEvents
            {
                // Let SignalR clients pass the JWT via the access_token query string.
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    if (!string.IsNullOrEmpty(accessToken)
                        && context.HttpContext.Request.Path.StartsWithSegments("/audioHub"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
                // Reject access tokens that were revoked at logout. Runs AFTER signature
                // validation, replacing the old pre-routing middleware that trusted unsigned tokens.
                OnTokenValidated = async context =>
                {
                    var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                    if (jti == null)
                    {
                        context.Fail("invalid_token");
                        return;
                    }

                    var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                    var isRevoked = await dbContext.AccessTokens
                        .Where(x => x.Jti == jti)
                        .Select(x => (bool?)x.IsRevoked)
                        .FirstOrDefaultAsync(context.HttpContext.RequestAborted);

                    // Unknown jti (cleaned up) or explicitly revoked → reject.
                    if (isRevoked != false)
                    {
                        context.Fail("token_revoked");
                    }
                }
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

static void ConfigureAppSettings(IServiceCollection services, IConfiguration configuration)
{

    services.AddOptions<ImageStoreOptions>().BindConfiguration("ImageStore");

    services.AddOptions<AuthOptions>().BindConfiguration(nameof(AuthOptions));

    services.AddOptions<AudioStoreOptions>().BindConfiguration("AudioStore");
}