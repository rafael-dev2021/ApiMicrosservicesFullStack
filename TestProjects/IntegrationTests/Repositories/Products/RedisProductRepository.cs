using ApiMicrosservicesProduct.Models;
using ApiMicrosservicesProduct.Models.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace TestProjects.IntegrationTests.Repositories.Products;

public class RedisProductRepository(IDatabase cache) : IProductRepository
{
    private readonly IDatabase _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public async Task<Product> CreateAsync(Product entity)
    {
        var serializedEntity = JsonSerializer.Serialize(entity);
        await _cache.StringSetAsync($"Product:{entity.Id}", serializedEntity);
        return entity;
    }

    public async Task<Product> GetByIdAsync(int? id)
    {
        if (id.HasValue)
        {
            var product = await _cache.StringGetAsync($"Product:{id}");
            if (!product.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<Product>(product);
            }
        }
        return null;
    }

    public async Task<IEnumerable<Product>> GetItemsAsync()
    {
        var products = new List<Product>();

        // Replace this loop logic with your actual retrieval logic
        for (int i = 1; i <= 2; i++)
        {
            var product = await _cache.StringGetAsync($"Product:{i}");
            if (!product.IsNullOrEmpty)
            {
                var productObject = JsonSerializer.Deserialize<Product>(product);
                products.Add(productObject);
            }
        }

        return products;
    }

    public Task<IEnumerable<Product>> GetProductsByCategoriesAsync(string categoryStr)
    {
        // Implement logic to retrieve products by category using Redis
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> GetSearchProductAsync(string keyword)
    {
        throw new NotImplementedException();
    }

    public async Task<Product> RemoveAsync(Product entity)
    {
        var product = await GetByIdAsync(entity.Id);
        if (product != null)
        {
            await _cache.KeyDeleteAsync($"Product:{entity.Id}");
        }
        return product;
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        var product = await GetByIdAsync(entity.Id);
        if (product != null)
        {
            var serializedEntity = JsonSerializer.Serialize(entity);
            await _cache.StringSetAsync($"Product:{entity.Id}", serializedEntity);
            return entity;
        }
        return null;
    }
}
