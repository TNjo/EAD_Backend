using BackendServices.Models;

namespace BackendServices;

// public interface IOrderRepository
// {
//     Task CreateOrderAsync(Order order);
//     Task<Order> GetOrderByIdAsync(string orderId);
//     Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId);
//     Task<List<Order>> GetOrdersByVendorEmailAsync(string vendorEmail);
//     Task UpdateOrderAsync(Order order);
//     Task DeleteOrderAsync(string orderId);
// }


public interface IOrderRepository
{
    // Create a new order
    Task CreateOrderAsync(Order order);

    // Get order by ID
    Task<Order> GetOrderByIdAsync(string orderId);

    // Update an existing order
    Task UpdateOrderAsync(Order order);
    Task<List<Order>> GetOrdersByVendorEmailAsync(string vendorEmail);

    // Get all orders for a customer
    // Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId);
    //
    // // Get all orders (for Admin or CSR)
    // Task<List<Order>> GetAllOrdersAsync();
    //
    // // Get orders that contain a specific product (e.g., for inventory management)
    // Task<List<Order>> GetOrdersByProductIdAsync(string productId);
    //
    // // Get all pending orders for a specific vendor
    // Task<List<Order>> GetPendingOrdersByVendorEmailAsync(string vendorEmail);
}