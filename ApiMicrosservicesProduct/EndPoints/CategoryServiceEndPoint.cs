using ApiMicrosservicesProduct.DTOs;
using ApiMicrosservicesProduct.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ApiMicrosservicesProduct.EndPoints;
public static class CategoryServiceEndPoint
{
    public static void MapCategoryServiceEndpoints(this WebApplication app)
    {
        app.MapGet("/api/v1/getcategories", async ([FromServices] ICategoryDtoService service, IDistributedCache cache) =>
        {
            var cachedCategories = await cache.GetStringAsync("cached_categories");

            if (!string.IsNullOrEmpty(cachedCategories))
            {
                var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(cachedCategories);
                return Results.Ok(categories);
            }
            else
            {
                var categories = await service.GetItemsDtoAsync();

                if (categories == null || !categories.Any())
                {
                    return Results.NotFound("No categories found");
                }
                else
                {
                    var serializedCategories = JsonConvert.SerializeObject(categories);
                    await cache.SetStringAsync("cached_categories", serializedCategories, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
                    });

                    return Results.Ok(categories);
                }
            }
        });

        
        app.MapGet("/api/v1/getcategorybyid/{id}", async ([FromServices] ICategoryDtoService service, IDistributedCache cache, int? id) =>
        {
            var cachedCategory = await cache.GetStringAsync($"cached_category_{id}");

            if (!string.IsNullOrEmpty(cachedCategory))
            {
                var category = JsonConvert.DeserializeObject<CategoryDto>(cachedCategory);
                return Results.Ok(category);
            }
            else
            {
                var category = await service.GetByIdAsync(id);

                if (category == null)
                {
                    return Results.NotFound("Category not found");
                }
                else
                {
                    var serializedCategory = JsonConvert.SerializeObject(category);
                    await cache.SetStringAsync($"cached_category_{id}", serializedCategory, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
                    });

                    return Results.Ok(category);
                }
            }
        });

        
        app.MapPost("/api/v1/addcategory", async ([FromServices] ICategoryDtoService service, [FromBody] CategoryDto categoryDto, [FromServices] IValidator<CategoryDto> validator) =>
        {
            if (categoryDto == null) return Results.BadRequest("Invalid category data.");

            var validationResult = await validator.ValidateAsync(categoryDto);
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);

            if (!validationResult.IsValid) return Results.BadRequest(errors);

            try
            {
                await service.AddAsync(categoryDto);
                return Results.Created($"/api/v1/addcategory/{categoryDto.Id}", categoryDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest("An error occurred while adding the category: " + ex.Message);
            }
        });

        
        app.MapPut("/api/v1/updatecategory/{id}", async ([FromServices] ICategoryDtoService service, int? id, [FromBody] CategoryDto updateCategoryDto, [FromServices] IValidator<CategoryDto> validator) =>
        {
            if (id != updateCategoryDto?.Id) return Results.BadRequest("ID mismatch between URL and category data.");

            if (updateCategoryDto == null) return Results.BadRequest("Invalid category data.");

            var validationResult = await validator.ValidateAsync(updateCategoryDto);
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);

            if (!validationResult.IsValid) return Results.BadRequest(errors);

            try
            {
                await service.UpdateAsync(updateCategoryDto);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest("An error occurred while updating the category: " + ex.Message);
            }
        });

        
        app.MapDelete("/api/v1/deletecategory/{id}", async (int? id, [FromServices] ICategoryDtoService service) =>
        {
            if (id == null) return Results.NotFound("Category ID is missing.");

            var category = await service.GetByIdAsync(id);
            if (category == null) return Results.NotFound($"Category with ID {id} not found.");

            await service.DeleteAsync(id.Value);
            return Results.NoContent();
        });

    }
}