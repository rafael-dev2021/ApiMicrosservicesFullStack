using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMicrosservicesProduct.DTOs;
public record ProductDto
{
    public int Id { get; set; }

    [DisplayName("Name")]
    public string Name { get; set; } = string.Empty;
    public List<string> Images { get; set; } = [];

    [DisplayName("Description")]
    public string Description { get; set; } = string.Empty;

    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    [Range(1, 9999)]
    [DisplayName("Price")]
    public decimal Price { get; set; }

    [Range(1, 9999)]
    [DisplayName("Stock")]
    public int Stock { get; set; }
    public string CategoryName { get; set; }

    [DisplayName("Categories")]
    public int CategoryId { get; set; }
}