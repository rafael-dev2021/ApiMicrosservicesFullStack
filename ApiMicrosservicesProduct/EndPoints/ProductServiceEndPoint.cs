using ApiMicrosservicesProduct.DTOs;
using ApiMicrosservicesProduct.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ApiMicrosservicesProduct.EndPoints;

public static class ProductServiceEndPoint
{
    public static void MapProductServiceEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/getproducts", async ([FromServices] IProductDtoService service, IDistributedCache cache) =>
        {
            var cachedProducts = await cache.GetStringAsync("cached_products");

            if (!string.IsNullOrEmpty(cachedProducts))
            {
                var products = JsonConvert.DeserializeObject<List<ProductDto>>(cachedProducts);
                return Results.Ok(products);
            }
            else
            {
                var products = await service.GetItemsDtoAsync();

                if (products == null || !products.Any())
                {
                    return Results.NotFound("No products found");
                }
                else
                {
                    var serializedProducts = JsonConvert.SerializeObject(products);
                    await cache.SetStringAsync("cached_products", serializedProducts, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
                    });

                    return Results.Ok(products);
                }
            }
        });


        app.MapGet("/api/v1/productbyid/{id}", async ([FromServices] IProductDtoService service, IDistributedCache cache, int? id) =>
        {
            var cachedProduct = await cache.GetStringAsync($"product_{id}");

            if (!string.IsNullOrEmpty(cachedProduct))
            {
                var product = JsonConvert.DeserializeObject<ProductDto>(cachedProduct);
                return Results.Ok(product);
            }
            else
            {
                var product = await service.GetByIdAsync(id);

                if (product == null)
                {
                    return Results.NotFound("Product not found");
                }
                else
                {
                    var serializedProduct = JsonConvert.SerializeObject(product);
                    await cache.SetStringAsync($"product_{id}", serializedProduct, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
                    });

                    return Results.Ok(product);
                }
            }
        });

       
        app.MapGet("/api/v1/products/search/{keyword}", async ([FromServices] IProductDtoService service, IDistributedCache cache, string keyword) =>
        {
            IEnumerable<ProductDto> products;

            var cachedProducts = await cache.GetStringAsync($"cached_products_{keyword}");

            if (!string.IsNullOrEmpty(cachedProducts))
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(cachedProducts);
            }
            else
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    products = await service.GetItemsDtoAsync();
                }
                else
                {
                    products = await service.GetSearchProductDtoAsync(keyword);
                    if (!products.Any())
                        return Results.NotFound("Product not found");

                    var serializedProducts = JsonConvert.SerializeObject(products);
                    await cache.SetStringAsync($"cached_products_{keyword}", serializedProducts, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
                    });
                }
            }

            return Results.Ok(products);
        });


        app.MapPost("/api/v1/addproduct", async ([FromServices] IProductDtoService service, [FromBody] ProductDto productDto, [FromServices] IValidator<ProductDto> validator) =>
        {
            if (productDto == null) return Results.BadRequest("Invalid product data.");

            var validationResult = await validator.ValidateAsync(productDto);
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);

            if (!validationResult.IsValid) return Results.BadRequest(errors);

            try
            {
                await service.AddAsync(productDto);
                return Results.Created($"/api/v1/addproduct/{productDto.Id}", productDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest("An error occurred while adding the product: " + ex.Message);
            }
        });


        app.MapPut("/api/v1/updateproduct/{id}", async ([FromServices] IProductDtoService service, int? id, [FromBody] ProductDto updateProductDto, [FromServices] IValidator<ProductDto> validator) =>
        {
            if (id != updateProductDto?.Id) return Results.BadRequest("ID mismatch between URL and product data.");

            if (updateProductDto == null) return Results.BadRequest("Invalid product data.");

            var validationResult = await validator.ValidateAsync(updateProductDto);
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);

            if (!validationResult.IsValid) return Results.BadRequest(errors);

            try
            {
                await service.UpdateAsync(updateProductDto);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest("An error occurred while updating the product: " + ex.Message);
            }
        });


        app.MapDelete("/api/v1/deleteproduct/{id}", async ([FromServices] IProductDtoService service, int? id) =>
        {
            if (id == null) return Results.NotFound("Product ID is missing.");

            var product = await service.GetByIdAsync(id);
            if (product == null) return Results.NotFound($"Product with ID {id} not found.");

            await service.DeleteAsync(id.Value);
            return Results.NoContent();
        });

    }
}