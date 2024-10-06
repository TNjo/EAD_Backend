using System.ComponentModel.DataAnnotations.Schema;

namespace BackendServices.DTOs;

// DTOs/ProductDTO.cs
public class ProductDTO
{
    
    // public string Name { get; set; }
    // public string Description { get; set; }
    // public decimal Price { get; set; }
    
    
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public DateTime CreatedDate { get; set; }
}

    