using ApiMicrosservicesWeb.Models.MicrosservicesProduct;
using ApiMicrosservicesWeb.Services.MicrosservicesProduct.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ApiMicrosservicesWeb.Services.MicrosservicesProduct;
public class CategoryService : ICategoryService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions _options;
    private const string apiEndpoint = "/api/v1/categories";

    public CategoryService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
    private static void PutTokenInHeaderAuthorization(string token, HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization =
                   new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<IEnumerable<CategoryViewModel>> GetAllCategories(string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        IEnumerable<CategoryViewModel> categories;

        using (var response = await client.GetAsync(apiEndpoint))
        {

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                categories = await JsonSerializer
                          .DeserializeAsync<IEnumerable<CategoryViewModel>>(apiResponse, _options);
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }
        return categories;
    }

    public async Task<CategoryViewModel> GetByCategoryIdAsync(int id, string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        CategoryViewModel category;

        using (var response = await client.GetAsync($"{apiEndpoint}/{id}"))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                category = await JsonSerializer.DeserializeAsync<CategoryViewModel>(apiResponse, _options);
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }

        return category;
    }

    public async Task<CategoryViewModel> CreateCategoryAsync(CategoryViewModel categoryViewModel, string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        PutTokenInHeaderAuthorization(token, client);

        StringContent content = new(JsonSerializer.Serialize(categoryViewModel), Encoding.UTF8, "application/json");

        using var response = await client.PostAsync(apiEndpoint, content);

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<CategoryViewModel>(apiResponse, _options);
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }

    public async Task<CategoryViewModel> UpdateCategoryAsync(CategoryViewModel categoryViewModel, string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        PutTokenInHeaderAuthorization(token, client);

        using var response = await client.PutAsJsonAsync($"{apiEndpoint}/{categoryViewModel.Id}", categoryViewModel);

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(apiResponse) && IsJson(apiResponse))
            {
                return JsonSerializer.Deserialize<CategoryViewModel>(apiResponse, _options);
            }
            else
            {
                return null;
            }
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }

    private bool IsJson(string input)
    {
        input = input.Trim();
        return input.StartsWith("{") && input.EndsWith("}")
               || input.StartsWith("[") && input.EndsWith("]");
    }


    public async Task<bool> DeleteCategoryAsync(int? id, string token)
    {
        var client = _clientFactory.CreateClient("ProductApi");
        PutTokenInHeaderAuthorization(token, client);

        using var response = await client.DeleteAsync($"{apiEndpoint}/{id}");

        return response.IsSuccessStatusCode;
    }
}