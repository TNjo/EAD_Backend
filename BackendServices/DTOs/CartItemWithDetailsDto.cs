namespace BackendServices.DTOs;

public class CartItemWithDetailsDto
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
}