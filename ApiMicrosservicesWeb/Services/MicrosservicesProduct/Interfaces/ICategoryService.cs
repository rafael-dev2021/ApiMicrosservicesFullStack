using ApiMicrosservicesWeb.Models.MicrosservicesProduct;

namespace ApiMicrosservicesWeb.Services.MicrosservicesProduct.Interfaces;
public interface ICategoryService
{
    Task<IEnumerable<CategoryViewModel>> GetAllCategories(string token);
    Task<CategoryViewModel> GetByCategoryIdAsync(int id, string token);
    Task<CategoryViewModel> CreateCategoryAsync(CategoryViewModel categoryViewModel, string token);
    Task<CategoryViewModel> UpdateCategoryAsync(CategoryViewModel categoryViewModel, string token);
    Task<bool> DeleteCategoryAsync(int? id, string token);
}
