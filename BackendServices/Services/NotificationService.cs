using BackendServices.Models;

namespace BackendServices.Services;

public class NotificationService : INotificationService
{
    // Notify vendor about a new order
    public async Task NotifyVendorNewOrderAsync(string vendorEmail, Order order)
    {
        var subject = $"New Order Received - Order #{order.Id}";
        var message = $"You have received a new order. Please check your dashboard for details.";
        await SendNotificationAsync(vendorEmail, subject, message);
    }

    // Notify customer about order cancellation
    public async Task NotifyCustomerOrderCanceledAsync(string customerId, Order order, string cancellationNote)
    {
        var subject = $"Order #{order.Id} Canceled";
        var message = $"Your order has been canceled. Reason: {cancellationNote}";
        await SendNotificationAsync(customerId, subject, message);
    }

    // Notify customer about order delivery
    public async Task NotifyCustomerOrderDeliveredAsync(string customerId, Order order)
    {
        var subject = $"Order #{order.Id} Delivered";
        var message = $"Your order has been successfully delivered. Thank you for shopping with us!";
        await SendNotificationAsync(customerId, subject, message);
    }

    // Notify vendor about low stock
    public async Task NotifyVendorLowStockAsync(string vendorEmail, Product product)
    {
        var subject = $"Low Stock Alert for {product.ProductName}";
        var message = $"Your product '{product.ProductName}' is low on stock. Only {product.AvailableQuantity} items remaining.";
        await SendNotificationAsync(vendorEmail, subject, message);
    }

    // Send generic notifications
    public async Task SendNotificationAsync(string recipientEmail, string subject, string message)
    {
        // Logic to send email/SMS/notification
        await Task.CompletedTask;
        // For example, if using email service, it would look something like:
        // await _emailSender.SendEmailAsync(recipientEmail, subject, message);
    }
}

