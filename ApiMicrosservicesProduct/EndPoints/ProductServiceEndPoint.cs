using ApiMicrosservicesProduct.DTOs;
using ApiMicrosservicesProduct.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ApiMicrosservicesProduct.EndPoints;

public static class ProductServiceEndPoint
{
    public static void MapProductServiceEndpoints(this WebApplication app)
    {
        /// <summary>
        /// Endpoint para obter todos os produtos.
        /// </summary>
        /// <param name="service">Serviço de operações de produto.</param>
        /// <returns>Um resultado HTTP com os produtos encontrados ou uma mensagem indicando que nenhum produto foi encontrado.</returns>
        app.MapGet("/api/v1/getproducts", async ([FromServices] IProductDtoService service) =>
        {
            var products = await service.GetItemsDtoAsync();

            if (products == null || !products.Any()) return Results.NotFound("No products found");

            return Results.Ok(products);
        });


        /// <summary>
        /// Endpoint para obter um produto por ID.
        /// </summary>
        /// <param name="service">Serviço de operações de produto.</param>
        /// <param name="id">ID do produto a ser obtido.</param>
        /// <returns>Um resultado HTTP com o produto encontrado ou uma mensagem indicando que o produto não foi encontrado.</returns>
        app.MapGet("/api/v1/petproductbyid/{id}", async ([FromServices] IProductDtoService service, int? id) =>
        {
            var product = await service.GetByIdAsync(id);

            return product == null
                ? Results.NotFound("Product not found")
                : Results.Ok(product);
        });


        /// <summary>
        /// Endpoint para buscar produtos com base em uma palavra-chave.
        /// </summary>
        /// <param name="service">Serviço de operações de produto.</param>
        /// <param name="keyword">Palavra-chave para buscar produtos.</param>
        /// <returns>Um resultado HTTP com os produtos encontrados ou uma mensagem indicando que nenhum produto foi encontrado.</returns>
        app.MapGet("/api/v1/products/search/{keyword}", async ([FromServices] IProductDtoService service, string keyword) =>
        {
            IEnumerable<ProductDto> products;
            if (string.IsNullOrEmpty(keyword))
                products = await service.GetItemsDtoAsync();

            else
            {
                products = await service.GetSearchProductDtoAsync(keyword);
                if (!products.Any()) return Results.NotFound("Product not found");
            }

            return Results.Ok(products);
        });


        /// <summary>
        /// Endpoint para adicionar um novo produto.
        /// </summary>
        /// <param name="service">Serviço de operações de produto.</param>
        /// <param name="productDto">Dados do produto a serem adicionados.</param>
        /// <param name="validator">Validador para validar o produto.</param>
        /// <returns>Um resultado HTTP indicando o status da operação.</returns>
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


        /// <summary>
        /// Endpoint para atualizar um produto pelo ID.
        /// </summary>
        /// <param name="service">Serviço de operações de produto.</param>
        /// <param name="id">ID do produto a ser atualizado.</param>
        /// <param name="updateProductDto">Dados do produto a serem atualizados.</param>
        /// <param name="validator">Validador para validar o produto.</param>
        /// <returns>Um resultado HTTP indicando o status da operação.</returns>
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



        /// <summary>
        /// Endpoint para deletar um produto pelo ID.
        /// </summary>
        /// <param name="service">Serviço de operações de produto.</param>
        /// <param name="id">ID do produto a ser deletado.</param>
        /// <returns>Um resultado HTTP indicando o status da operação.</returns>
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