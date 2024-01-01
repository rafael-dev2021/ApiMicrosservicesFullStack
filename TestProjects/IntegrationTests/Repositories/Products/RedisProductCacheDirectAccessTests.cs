using ApiMicrosservicesProduct.Models;
using ApiMicrosservicesProduct.Models.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace TestProjects.IntegrationTests.Repositories.Products
{
    public class RedisProductCacheDirectAccessTests
    {
        private static readonly string[] sourceArray1 = ["Image1.jpg"];
        private static readonly string[] sourceArray2 = ["Image2.jpg"];
        private static readonly string[] sourceArray3 = ["Image3.jpg"];
        private static readonly string[] sourceArray4 = ["Image4.jpg"];

        [Fact]
        public async Task GetItemsAsync_ReturnsProductsFromRedis()
        {
            // Create test products
            var productTest1 = new Product(1, "Product 1", [.. sourceArray1], "Description 1", 10.0m, 100, 1);
            var productTest2 = new Product(2, "Product 2", [.. sourceArray2], "Description 2", 20.0m, 50, 2);
            var productTest3 = new Product(3, "Product 3", [.. sourceArray3], "Description 3", 15.0m, 75, 1);

            // Store test products in the Redis cache
            await AddProductsToRedis(productTest1, productTest2, productTest3);

            // Create an instance of the Redis repository
            var repository = GetRedisRepository();
            var products = await repository.GetItemsAsync();

            // Assert
            Assert.NotNull(products);
            Assert.Equal(2, products.Count()); // Check if three products were returned

            // Clear Redis data after the test
            await ClearProductFromRedis(productTest1.Id);
            await ClearProductFromRedis(productTest2.Id);
        }
        [Fact]
        public async Task CreateAsync_AddsProduct()
        {
            var repository = GetRedisRepository();
            var productToAdd = new Product(4, "Product 4", [.. sourceArray4], "Description 4", 25.0m, 80, 2);
            await repository.CreateAsync(productToAdd);

            var addedProduct = await repository.GetByIdAsync(4);

            // Assert
            Assert.NotNull(addedProduct);
            Assert.Equal(4, addedProduct.Id); // Check if the product was added correctly with ID 4
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectProduct()
        {
            var repository = GetRedisRepository();
            var product = await repository.GetByIdAsync(4);

            // Assert
            Assert.NotNull(product);
            product.UpdateNameUnitTest("Updated Product");
        }


        [Fact]
        public async Task UpdateAsync_UpdatesProduct()
        {
            var repository = GetRedisRepository();

            var existingProduct = await repository.GetByIdAsync(4);

            if (existingProduct != null)
            {
                existingProduct.UpdateProductUnitTest("Updated Product", [.. sourceArray1], "Updated Description", 30.0m, 90, 2);

                var result = await repository.UpdateAsync(existingProduct);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Updated Product", result.Name); // Verificar se o nome do produto foi atualizado
                                                              // Adicionar mais verificações para outras propriedades atualizadas, se necessário
            }
            else
            {
                throw new Exception("Product not found.");
            }
        }


        [Fact]
        public async Task RemoveAsync_RemovesProduct()
        {
            var repository = GetRedisRepository();
            var productToRemove = await repository.GetByIdAsync(3);
            var removedProduct = await repository.RemoveAsync(productToRemove);

            var deletedProduct = await repository.GetByIdAsync(3);

            // Assert
            Assert.NotNull(removedProduct);
            Assert.Null(deletedProduct); // Check if the product was removed correctly
        }



        private IProductRepository GetRedisRepository()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var cache = redis.GetDatabase();
            return new RedisProductRepository(cache);
        }

        private async Task AddProductsToRedis(params Product[] products)
        {
            using var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var cache = redis.GetDatabase();

            foreach (var product in products)
            {
                var serializedEntity = JsonSerializer.Serialize(product);
                await cache.StringSetAsync($"Product:{product.Id}", serializedEntity);
            }
        }

        private async Task ClearProductFromRedis(int productId)
        {
            using var redis = ConnectionMultiplexer.Connect("localhost:6379");
            var cache = redis.GetDatabase();

            await cache.KeyDeleteAsync($"Product:{productId}");
        }
    }
}
