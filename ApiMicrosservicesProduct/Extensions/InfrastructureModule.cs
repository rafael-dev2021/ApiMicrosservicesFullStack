namespace ApiMicrosservicesProduct.Extensions;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
          .AddDataBaseDependecyInjection(configuration)
          .AddAuthenticationDependecyInjection(configuration)
          .AddRepositoryDependecyInjection()
          .AddServiceDependecyInjection()
          .AddFluentValidationDependecyInjection();
        return services;
    }
}
