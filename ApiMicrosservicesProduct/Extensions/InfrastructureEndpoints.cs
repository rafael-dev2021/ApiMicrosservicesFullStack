using ApiMicrosservicesProduct.EndPoints;

namespace ApiMicrosservicesProduct.Extensions
{
    public static class InfrastructureEndpoints
    {
        public static void MapInfrastructureEndpoints(this WebApplication app)
        {
            app.MapCategoryServiceEndpoints();
            app.MapProductServiceEndpoints();
        }
        public static IServiceCollection EndpointsApiExplorerDI(this IServiceCollection services)
        {
            return services.AddEndpointsApiExplorer();
        }
    }
}
