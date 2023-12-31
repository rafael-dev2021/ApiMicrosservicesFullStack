using ApiMicrosservicesProduct.DTOs;

namespace ApiMicrosservicesProduct.Services.Interfaces;
public interface IProductDtoService : IGenericService<ProductDto>
{
    Task<IEnumerable<ProductDto>> GetProductsDtoByCategoriesAsync(string categoryStr);
    Task<IEnumerable<ProductDto>> GetSearchProductDtoAsync(string keyword);
}