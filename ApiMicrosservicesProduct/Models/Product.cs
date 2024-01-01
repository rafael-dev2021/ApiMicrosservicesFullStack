using System.Text.Json.Serialization;

namespace ApiMicrosservicesProduct.Models;
public class Product
{
    public Product() { }

    public Product(int id, string name, List<string> images, string description, decimal price, int stock, int categoryId)
    {
        Id = id;
        Name = name;
        Images = images;
        Description = description;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
    }

    public void UpdateProductUnitTest(string name, List<string> images, string description, decimal price, int stock, int categoryId)
    {
        Name = name;
        Images = images;
        Description = description;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
    }
    public void UpdateNameUnitTest(string newName)
    {
        Name = newName;
    }

    public int Id { get; set; }
    public string Name { get; protected set; } = string.Empty;
    public List<string> Images { get; protected set; } = [];
    public string Description { get; protected set; } = string.Empty;
    public decimal Price { get; protected set; }
    public int Stock { get; protected set; }

    [JsonIgnore]
    public Category Category { get; protected set; }
    public int CategoryId { get; protected set; }
}