using BackendServices.DTOs;
using BackendServices.Models;

namespace BackendServices.Services;

// public class OrderService
// {
//     private readonly IOrderRepository _orderRepository;
//     private readonly UserService _userService; // For notifications
//
//     public OrderService(IOrderRepository orderRepository, UserService userService)
//     {
//         _orderRepository = orderRepository;
//         _userService = userService;
//     }
//
//     // Create a new order
//     public async Task CreateOrderAsync(OrderDTO orderDto)
//     {
//         var order = new Order
//         {
//             CustomerId = orderDto.CustomerId,
//             Items = orderDto.Items.Select(itemDto => new OrderItem
//             {
//                 ProductId = itemDto.ProductId,
//                 Quantity = itemDto.Quantity,
//                 ItemStatus = "processing"
//             }).ToList(),
//             TotalAmount = 0, // Calculation of total will be done here
//             Status = "processing",
//             CreatedDate = DateTime.UtcNow
//         };
//
//         // Notify vendor about the new order
//         foreach (var item in order.Items)
//         {
//             await _userService.NotifyVendorAsync(item.VendorEmail, "New order placed for your product.");
//         }
//
//         await _orderRepository.CreateOrderAsync(order);
//     }
//
//     // Get all orders for a customer
//     public async Task<List<Order>> GetOrdersByCustomerAsync(string customerId)
//     {
//         return await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
//     }
//
//     // Get orders for a specific vendor
//     public async Task<List<Order>> GetOrdersByVendorAsync(string vendorEmail)
//     {
//         return await _orderRepository.GetOrdersByVendorEmailAsync(vendorEmail);
//     }
//
//     // Vendor updates the status of their product in the order
//     public async Task UpdateProductStatusAsync(string orderId, string productId, string vendorEmail, string status)
//     {
//         var order = await _orderRepository.GetOrderByIdAsync(orderId);
//         var item = order.Items.FirstOrDefault(i => i.ProductId == productId && i.VendorEmail == vendorEmail);
//
//         if (item != null)
//         {
//             item.ItemStatus = status;
//
//             // Check if all items are delivered to update the overall order status
//             if (order.Items.All(i => i.ItemStatus == "delivered"))
//             {
//                 order.Status = "delivered";
//                 await _userService.NotifyCustomerAsync(order.CustomerId, "Your order has been fully delivered.");
//             }
//             else if (order.Items.Any(i => i.ItemStatus == "delivered"))
//             {
//                 order.Status = "partially_delivered";
//             }
//
//             await _orderRepository.UpdateOrderAsync(order);
//         }
//     }
//
//     // Cancel an order by customer or admin
//     public async Task CancelOrderAsync(string orderId, string note)
//     {
//         var order = await _orderRepository.GetOrderByIdAsync(orderId);
//         if (order != null && order.Status != "delivered")
//         {
//             order.Status = "cancelled";
//             order.Note = note;
//             await _orderRepository.UpdateOrderAsync(order);
//
//             // Notify customer
//             await _userService.NotifyCustomerAsync(order.CustomerId, "Your order has been cancelled.");
//         }
//     }
//
//     // Admin deletes an order
//     public async Task DeleteOrderAsync(string orderId)
//     {
//         await _orderRepository.DeleteOrderAsync(orderId);
//     }
// }



public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly INotificationService _notificationService;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IVendorRepository vendorRepository, INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _vendorRepository = vendorRepository;
        _notificationService = notificationService;
    }

    // Create a new order
    public async Task CreateOrderAsync(Order order)
    {
        // Validate product availability
        foreach (var item in order.Items)
        {
            var product = await _productRepository.GetProductByIdAsync(item.ProductId);
            if (product.AvailableQuantity < item.Quantity)
            {
                throw new InvalidOperationException($"Product {product.ProductName} does not have enough stock.");
            }

            // Reduce product stock
            product.AvailableQuantity -= item.Quantity;
            await _productRepository.UpdateProductAsync(product);
        }

        // Save the order
        await _orderRepository.CreateOrderAsync(order);

        // Notify vendors about the new order
        foreach (var item in order.Items)
        {
            // Get vendor email from the product or vendor data
            var vendor = await _vendorRepository.GetVendorByEmailAsync(item.VendorEmail);

            // Call notification service with the vendor's email, not the Vendor object
            await _notificationService.NotifyVendorNewOrderAsync(vendor.Email, order);
        }
    }


    // Update order status (e.g., before dispatch)
    public async Task UpdateOrderStatusAsync(string orderId, string status)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        order.Status = status;
        await _orderRepository.UpdateOrderAsync(order);
    }

    // Cancel an order (with note)
    public async Task CancelOrderAsync(string orderId, string cancellationNote)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null || order.Status == "Dispatched")
        {
            throw new InvalidOperationException("Order cannot be canceled once dispatched.");
        }

        order.Status = "Canceled";
        order.CustomerNote = cancellationNote;
        await _orderRepository.UpdateOrderAsync(order);

        // Notify the customer
        //await _notificationService.NotifyCustomerOrderCanceledAsync(order.CustomerId, order);
    }

    // Mark an order or item as delivered
    public async Task MarkOrderAsDelivered(string orderId, string vendorEmail = null)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }
    
        if (vendorEmail != null)
        {
            // Mark individual vendor's product as delivered
            var vendorStatus = order.VendorStatuses.FirstOrDefault(vs => vs.VendorEmail == vendorEmail);
            if (vendorStatus != null)
            {
                vendorStatus.Status = "Delivered";
            }
    
            // Check if all vendors have delivered
            if (order.VendorStatuses.All(vs => vs.Status == "Delivered"))
            {
                order.Status = "Delivered";
            }
            else
            {
                order.Status = "Partially Delivered";
            }
        }
        else
        {
            // Mark the entire order as delivered by CSR/Admin
            order.Status = "Delivered";
        }
    
        await _orderRepository.UpdateOrderAsync(order);
    
        // Notify the customer
        await _notificationService.NotifyCustomerOrderDeliveredAsync(order.CustomerId, order);
    }
    
    
    // Fetch orders by vendor email
    public async Task<List<Order>> GetOrdersByVendorEmailAsync(string vendorEmail)
    {
        return await _orderRepository.GetOrdersByVendorEmailAsync(vendorEmail);
    }
    
   
}
