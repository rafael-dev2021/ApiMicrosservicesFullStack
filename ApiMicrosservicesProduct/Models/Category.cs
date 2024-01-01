using System.Text.Json.Serialization;

namespace ApiMicrosservicesProduct.Models;
public sealed class Category(int id, string name, string image)
{
    public int Id { get; set; } = id;
    public string Name { get; private set; } = name;
    public string Image { get; private set; } = image;

    [JsonIgnore]
    public ICollection<Product> Products { get; } = [];
}