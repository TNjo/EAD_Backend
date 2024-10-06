namespace BackendServices.DTOs;

// public class OrderDTO
// {
//     public string CustomerId { get; set; }
//     public List<OrderItemDTO> Items { get; set; }
//     public string Note { get; set; } // For cancellation
// }
//
// public class OrderItemDTO
// {
//     public string ProductId { get; set; }
//     public int Quantity { get; set; }
// }


public class OrderDTO
{
    public List<OrderItemDTO> Items { get; set; }  // List of products in the order
    public string CustomerNote { get; set; }  // Optional note from the customer

    // Additional fields if needed, such as shipping address, payment method, etc.
}

public class OrderItemDTO
{
    public string ProductId { get; set; }  // Product identifier
    public int Quantity { get; set; }  // Quantity of the product ordered
}