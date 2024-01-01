using ApiMicrosservicesProduct.Models;
using ApiMicrosservicesProduct.Models.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace TestProjects.IntegrationTests.Repositories.Categories;

public class RedisCategoryCacheDirectAccessTests
{
    [Fact]
    public async Task GetItemsAsync_ReturnsCategoriesFromRedis()
    {
        // Crie suas categorias de teste
        var categoryTest1 = new Category(1, "Teste 1", "Image1.jpg");
        var categoryTest2 = new Category(2, "Teste 2", "Image2.jpg");
        var categoryTest3 = new Category(3, "Teste 3", "Image3.jpg");

        // Armazene suas categorias no cache do Redis
        await AddCategoriesToRedis(categoryTest1, categoryTest2, categoryTest3);

        // Crie a instância do repository para Redis
        var repository = GetRedisRepository();
        var categories = await repository.GetItemsAsync();

        // Assert
        Assert.NotNull(categories);
        Assert.Equal(2, categories.Count()); // Verifica se duas categorias foram retornadas

        // Limpe os dados do Redis após o teste
        await ClearCategoryFromRedis(categoryTest1.Id);
        await ClearCategoryFromRedis(categoryTest2.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectCategory()
    {
        var repository = GetRedisRepository();
        var category = await repository.GetByIdAsync(4);

        // Assert
        Assert.NotNull(category);
        Assert.Equal("Teste 4", category.Name);
    }


    [Fact]
    public async Task CreateAsync_AddsCategory()
    {
        var repository = GetRedisRepository();
        var categoryToAdd = new Category(4, "Teste 4", "Image4.jpg");
        await repository.CreateAsync(categoryToAdd);

        var addedCategory = await repository.GetByIdAsync(4);

        // Assert
        Assert.NotNull(addedCategory);
        Assert.Equal(4, addedCategory.Id); // Verifica se a categoria foi adicionada corretamente com o ID 4
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCategory()
    {
        var repository = GetRedisRepository();

        var categoryToUpdate = await repository.GetByIdAsync(4);

        // Crie uma nova instância da categoria com o nome atualizado
        var updatedCategory = new Category(categoryToUpdate.Id, "Update", categoryToUpdate.Image);

        await repository.UpdateAsync(updatedCategory);

        var retrievedUpdatedCategory = await repository.GetByIdAsync(4); // Corrigido para ID 4

        // Assert
        Assert.NotNull(retrievedUpdatedCategory);
        Assert.Equal("Update", retrievedUpdatedCategory.Name); // Verifica se o nome da categoria foi atualizado
    }


    [Fact]
    public async Task RemoveAsync_RemovesCategory()
    {
        var repository = GetRedisRepository();
        var categoryToRemove = await repository.GetByIdAsync(3);
        var removedCategory = await repository.RemoveAsync(categoryToRemove);

        var deletedCategory = await repository.GetByIdAsync(3);

        // Assert
        Assert.NotNull(removedCategory);
        Assert.Null(deletedCategory); // Verifica se a categoria foi removida corretamente
    }

    private ICategoryRepository GetRedisRepository()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        var cache = redis.GetDatabase();
        return new RedisCategoryRepository(cache);
    }

    private async Task AddCategoriesToRedis(params Category[] categories)
    {
        using var redis = ConnectionMultiplexer.Connect("localhost:6379");
        var cache = redis.GetDatabase();

        foreach (var category in categories)
        {
            var serializedEntity = JsonSerializer.Serialize(category);
            await cache.StringSetAsync($"Category:{category.Id}", serializedEntity);
        }
    }

    private async Task ClearCategoryFromRedis(int categoryId)
    {
        using var redis = ConnectionMultiplexer.Connect("localhost:6379");
        var cache = redis.GetDatabase();

        await cache.KeyDeleteAsync($"Category:{categoryId}");
    }
}
