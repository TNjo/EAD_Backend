using BackendServices.Models;

namespace BackendServices.Services;

public interface INotificationService
{
    // Notify vendor about a new order
    Task NotifyVendorNewOrderAsync(string vendorEmail, Order order);

    // Notify customer about their order being canceled
    Task NotifyCustomerOrderCanceledAsync(string customerId, Order order, string cancellationNote);

    // Notify customer about their order being delivered
    Task NotifyCustomerOrderDeliveredAsync(string customerId, Order order);

    // Notify vendor about low stock for a product
    Task NotifyVendorLowStockAsync(string vendorEmail, Product product);

    // Send generic notifications (used internally)
    Task SendNotificationAsync(string recipientEmail, string subject, string message);
}
