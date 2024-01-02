namespace ApiMicrosservicesIdentityServer.Extensions;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.
             AddDatabaseIdentityDependecyInjection(configuration)
             .AddIdentityRulesDependecyInjection()
             .AddProfileAppServiceDependecyInjection()
             .AddSession()
             .AddMemoryCache();

        return services;
    }
}
