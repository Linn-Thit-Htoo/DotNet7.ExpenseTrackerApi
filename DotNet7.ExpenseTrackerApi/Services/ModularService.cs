using DotNet7.ExpenseTrackerApi.DbService;
using DotNet7.ExpenseTrackerApi.Middleware;
using DotNet7.ExpenseTrackerApi.Shared;
using Microsoft.EntityFrameworkCore;

namespace DotNet7.ExpenseTrackerApi.Services;

public static class ModularService
{
    public static IServiceCollection AddServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddCustomServices();
        services.AddDbContextService(builder);
        return services;
    }

    #region Add Authorization Middleware

    public static IApplicationBuilder AddAuthorizationMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<AuthorizationMiddleware>();
        return app;
    }

    #endregion

    #region Add Custom Services

    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<AdoDotNetService>();
        services.AddSingleton<AesService>();
        services.AddScoped<JwtService>();

        return services;
    }

    #endregion

    #region Add Db Context Service

    public static IServiceCollection AddDbContextService(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
        });

        return services;
    }

    #endregion
}