using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendServices.Models;

// public class Order
// {
//     public string Id { get; set; }
//     public string CustomerId { get; set; } // The user who placed the order
//     public List<OrderItem> Items { get; set; } // The products in the order
//     public decimal TotalAmount { get; set; } // Total price of the order
//     public string Status { get; set; } // Order status (e.g., processing, partially_delivered, delivered, cancelled)
//     public DateTime CreatedDate { get; set; } 
//     public DateTime? DispatchedDate { get; set; } // Date when dispatched
//     public string Note { get; set; } // Reason for cancellation
// }


// public class Order
// {
//     [BsonId]
//     [BsonRepresentation(BsonType.ObjectId)]
//     public string Id { get; set; }
//
//     public string CustomerId { get; set; }
//     public List<OrderItem> Items { get; set; }  // List of products
//     public string Status { get; set; } = "Processing";  // "Processing", "Dispatched", "Delivered", "Canceled"
//     public DateTime OrderDate { get; set; } = DateTime.UtcNow;
//     public string CustomerNote { get; set; }
//     public List<VendorStatus> VendorStatuses { get; set; } = new List<VendorStatus>();  // Status per vendor
//
//     public bool IsPartiallyDelivered
//     {
//         get
//         {
//             return VendorStatuses.Any(vs => vs.Status == "Delivered") && VendorStatuses.Any(vs => vs.Status != "Delivered");
//         }
//     }
// }
//
// public class OrderItem
// {
//     public string ProductId { get; set; }
//     public int Quantity { get; set; }
//     public string VendorEmail { get; set; }  // Vendor for the product
// }
//
public class VendorStatus
{
    public string VendorEmail { get; set; }
    public string Status { get; set; } = "Not Ready";  // "Not Ready", "Ready", "Delivered"
}



public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
 
    public string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();  // List of products
 
    // The status will be set based on the status of the order items.
    public string Status { get; set; } = "Processing";  // Default status
 
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string CustomerNote { get; set; }
     public List<VendorStatus> VendorStatuses { get; set; } = new List<VendorStatus>();  // Status per vendor
 
    // Method to update the OrderStatus based on OrderItem statuses
    public void UpdateOrderStatus()
    {
        // Check if all items are "Partially Delivered"
        if (Items.All(item => item.OrderItemStatus == "Partially Delivered"))
        {
            Status = "Delivered";  // All items partially delivered means overall order is delivered
        }
        // Check if any item is "Partially Delivered"
        else if (Items.Any(item => item.OrderItemStatus == "Partially Delivered"))
        {
            Status = "Partially Delivered";  // Any partially delivered items means partial delivery
        }
        // Check if all items are "New" or null
        else if (Items.All(item => item.OrderItemStatus == "New" || string.IsNullOrEmpty(item.OrderItemStatus)))
        {
            Status = "Processing";  // Default to processing if all items are new or statuses are empty
        }
        // Default to "Processing" in other cases
        else
        {
            Status = "Processing";
        }
    }
}

public class OrderItem
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public string VendorEmail { get; set; }  // Vendor for the product
    // "New", "Processing", "Partially Delivered", "Delivered", etc.
    public string OrderItemStatus { get; set; } = "New";
}