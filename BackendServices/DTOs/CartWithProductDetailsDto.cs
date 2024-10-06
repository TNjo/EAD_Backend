namespace BackendServices.DTOs;

public class CartWithProductDetailsDto
{
    public string CustomerId { get; set; }
    public List<CartItemWithDetailsDto> Items { get; set; }
    public decimal TotalPrice { get; set; }
}