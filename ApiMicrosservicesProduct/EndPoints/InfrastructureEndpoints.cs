namespace ApiMicrosservicesProduct.EndPoints
{
    public static class InfrastructureEndpoints
    {
        public static void MapInfrastructureEndpoints(this WebApplication app)
        {
            app.MapCategoryServiceEndpoints();
            app.MapProductServiceEndpoints();
        }
    }
}
