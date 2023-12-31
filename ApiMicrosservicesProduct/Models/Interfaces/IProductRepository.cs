namespace ApiMicrosservicesProduct.Models.Interfaces;
public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetSearchProductAsync(string keyword);
    Task<IEnumerable<Product>> GetProductsByCategoriesAsync(string categoryStr);
}