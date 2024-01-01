using System.ComponentModel;

namespace ApiMicrosservicesProduct.DTOs;
public record CategoryDto
{
    public int Id { get; set; }

    [DisplayName("Name")]
    public string Name { get; set; } = string.Empty;

    [DisplayName("Image")]
    public string Image { get; set; } = string.Empty;
}