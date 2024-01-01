using ApiMicrosservicesProduct.Models;
using ApiMicrosservicesProduct.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiMicrosservicesProduct.Context.Repositories;
public class ProductRepository(AppDbContext appDbContext) : IProductRepository
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<IEnumerable<Product>> GetItemsAsync()
    {
        return await _appDbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }
    public async Task<Product> GetByIdAsync(int? id)
    {
        return await _appDbContext.Products
            .Include(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == id);
    }
    public async Task<Product> CreateAsync(Product entity)
    {
        _appDbContext.Add(entity);
        await _appDbContext.SaveChangesAsync();
        return entity;
    }
    public async Task<Product> RemoveAsync(Product entity)
    {
        _appDbContext.Remove(entity);
        await _appDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        _appDbContext.Update(entity);
        await _appDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<Product>> GetSearchProductAsync(string keyword)
    {
        var products = await _appDbContext.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .ToListAsync();

        var filteredProducts = products
            .Where(x =>
                x.Name.ToLower().Contains(keyword.ToLower()) ||
                x.Category.Name.ToLower().Contains(keyword.ToLower()))
            .OrderBy(x => x.Id);

        return filteredProducts;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoriesAsync(string categoryStr)
    {
        return await _appDbContext.Products
                 .AsNoTracking()
                 .Where(category => category.Category.Name.Equals(categoryStr))
                 .Include(category => category.Category)
                 .ToListAsync();
    }
}