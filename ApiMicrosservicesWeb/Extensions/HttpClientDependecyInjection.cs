using ApiMicrosservicesWeb.Services.MicrosservicesProduct.Interfaces;
using ApiMicrosservicesWeb.Services.MicrosservicesProduct;

namespace ApiMicrosservicesWeb.Extensions;

public static class HttpClientDependecyInjection
{
    public static IServiceCollection AddHttpClientDependecyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IProductService, ProductService>("ProductApi", c =>
        {
            c.BaseAddress = new Uri(configuration["ServiceUri:ProductApi"]);
            c.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            c.DefaultRequestHeaders.Add("Keep-Alive", "3600");
            c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-ProductApi");
        });

        return services;
    }
}
