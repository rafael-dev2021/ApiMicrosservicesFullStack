namespace ApiMicrosservicesWeb.Extensions
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthenticationDependecyInjection(configuration)
                .AddHttpClientDependecyInjection(configuration)
                .AddViewModelsDependecyInjection();


            return services;
        }
    }
}
