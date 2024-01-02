using ApiMicrosservicesWeb.Services.MicrosservicesProduct.Interfaces;
using ApiMicrosservicesWeb.Services.MicrosservicesProduct;

namespace ApiMicrosservicesWeb.Extensions;

public static class ViewModelsDependecyInjection
{
    public static IServiceCollection AddViewModelsDependecyInjection(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}
