using ApiMicrosservicesWeb.Models.MicrosservicesProduct;
using ApiMicrosservicesWeb.Services.MicrosservicesProduct.Interfaces;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;

namespace ApiMicrosservicesWeb.Services.MicrosservicesProduct;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions _options;
    private readonly IDistributedCache _cache;
    private const string apiEndpoint = "/api/v1/products/";
    private const string apiEndpointId = "/api/v1/products/{id}";
    private ProductViewModel productVM;

    public ProductService(IHttpClientFactory clientFactory, IDistributedCache cache)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _cache = cache;
    }
    private static void PutTokenInHeaderAuthorization(string token, HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization =
                   new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProducts(string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        PutTokenInHeaderAuthorization(token, client);


        using (var response = await client.GetAsync(apiEndpoint))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                var productsViewModels = await JsonSerializer.DeserializeAsync<IEnumerable<ProductViewModel>>(apiResponse, _options);

                return productsViewModels ?? Enumerable.Empty<ProductViewModel>();
            }
            else
            {
                return Enumerable.Empty<ProductViewModel>();
            }
        }
    }


    public async Task<ProductViewModel> GetByProductIdAsync(int id, string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        PutTokenInHeaderAuthorization(token, client);


        using (var response = await client.GetAsync(apiEndpointId.Replace("{id}", id.ToString())))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productVM = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
        return productVM;
    }

    public async Task<ProductViewModel> CreateProductAsync(ProductViewModel productViewModel, string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        PutTokenInHeaderAuthorization(token, client);


        StringContent content = new(JsonSerializer.Serialize(productViewModel), Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(apiEndpoint, content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                productViewModel = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
        return productViewModel;
    }

    public async Task<ProductViewModel> UpdateProductAsync(ProductViewModel productViewModel, string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        PutTokenInHeaderAuthorization(token, client);


        ProductViewModel productUpdated = null;

        using (var response = await client.PutAsJsonAsync(apiEndpointId.Replace("{id}", productViewModel.Id.ToString()), productViewModel))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(apiResponse))
                {
                    productUpdated = JsonSerializer.Deserialize<ProductViewModel>(apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
        return productUpdated;
    }


    public async Task<bool> DeleteProductAsync(int? id, string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        PutTokenInHeaderAuthorization(token, client);


        if (id.HasValue)
        {
            string deleteUrl = $"{apiEndpoint}{id}";

            using var response = await client.DeleteAsync(deleteUrl);
            if (response.IsSuccessStatusCode)
            {
                _ = await response.Content.ReadAsStreamAsync();
                return true;
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
        else
        {
            return false;
        }
    }

    public async Task<IEnumerable<ProductViewModel>> GetProductsByKeywordAsync(string keyword, string token)
    {
        return await GetProductsFromCacheOrApi(keyword, token);
    }
    private async Task<IEnumerable<ProductViewModel>> GetProductsFromCacheOrApi(string keyword, string token)
    {
        var cachedProducts = await _cache.GetStringAsync($"cached_products_{keyword}");

        if (!string.IsNullOrEmpty(cachedProducts))
        {
            return JsonSerializer.Deserialize<List<ProductViewModel>>(cachedProducts);
        }
        else
        {
            return await GetProductsFromApi(keyword, token);
        }
    }
    private async Task<IEnumerable<ProductViewModel>> GetProductsFromApi(string keyword, string token)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            throw new ArgumentException("Keyword is required for searching products.");
        }

        var client = _clientFactory.CreateClient("ProductApi");
        PutTokenInHeaderAuthorization(token, client);

        using var response = await client.GetAsync($"/api/v1/products/search/{keyword}");

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStreamAsync();
            var products = await JsonSerializer.DeserializeAsync<IEnumerable<ProductViewModel>>(apiResponse, _options);

            var serializedProducts = JsonSerializer.Serialize(products);
            await _cache.SetStringAsync($"cached_products_{keyword}", serializedProducts, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return products;
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new ArgumentException("Products not found for the given keyword.");
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }
}
