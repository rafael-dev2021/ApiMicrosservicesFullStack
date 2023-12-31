using ApiMicrosservicesProduct.DTOs;
using ApiMicrosservicesProduct.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ApiMicrosservicesProduct.EndPoints;
public static class CategoryServiceEndPoint
{
    public static void MapCategoryServiceEndpoints(this WebApplication app)
    {
        /// <summary>
        /// Obtém todas as categorias.
        /// </summary>
        /// <param name="service">Serviço de operações de categoria.</param>
        /// <returns>Um resultado HTTP com as categorias encontradas ou uma mensagem indicando que nenhuma categoria foi encontrada.</returns>
        app.MapGet("/api/v1/getcategories", async ([FromServices] ICategoryDtoService service) =>
        {
            var categories = await service.GetItemsDtoAsync();

            if (categories == null || !categories.Any()) return Results.NotFound("No categories found");

            return Results.Ok(categories);
        });

        /// <summary>
        /// Obtém uma categoria pelo ID.
        /// </summary>
        /// <param name="id">ID da categoria a ser obtida.</param>
        /// <param name="service">Serviço de operações de categoria.</param>
        /// <returns>Um resultado HTTP com a categoria encontrada ou uma mensagem indicando que a categoria não foi encontrada.</returns>
        app.MapGet("/api/v1/getcategorybyid/{id}", async ([FromServices] ICategoryDtoService service, int? id) =>
        {
            var category = await service.GetByIdAsync(id);

            return category == null
                ? Results.NotFound("Category not found")
                : Results.Ok(category);
        });

        /// <summary>
        /// Adiciona uma nova categoria.
        /// </summary>
        /// <param name="service">Serviço de operações de categoria.</param>
        /// <param name="categoryDto">Dados da categoria a serem adicionados.</param>
        /// <param name="validator">Validador para validar a categoria.</param>
        /// <returns>Um resultado HTTP indicando o status da operação.</returns>
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

        /// <summary>
        /// Atualiza uma categoria específica pelo ID.
        /// </summary>
        /// <param name="service">Serviço de operações de categoria.</param>
        /// <param name="id">ID da categoria a ser atualizada.</param>
        /// <param name="updateCategoryDto">Dados da categoria a serem atualizados.</param>
        /// <param name="validator">Validador para validar a categoria.</param>
        /// <returns>Um resultado HTTP indicando o status da operação.</returns>
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

        /// <summary>
        /// Deleta uma categoria pelo ID.
        /// </summary>
        /// <param name="id">ID da categoria a ser deletada.</param>
        /// <param name="service">Serviço de operações de categoria.</param>
        /// <returns>Um resultado HTTP indicando o status da operação.</returns>
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