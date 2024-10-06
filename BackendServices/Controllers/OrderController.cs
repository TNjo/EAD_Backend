using System.Security.Claims;
using BackendServices.DTOs;
using BackendServices.Models;
using BackendServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BackendServices.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class OrderController : ControllerBase
// {
//     private readonly OrderService _orderService;
//
//     public OrderController(OrderService orderService)
//     {
//         _orderService = orderService;
//     }
//
//     // Create a new order
//     [HttpPost("create")]
//     public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderDto)
//     {
//         await _orderService.CreateOrderAsync(orderDto);
//         return Ok("Order created successfully.");
//     }
//
//     // Get all orders for a customer
//     [HttpGet("customer/{customerId}")]
//     public async Task<IActionResult> GetOrdersByCustomer(string customerId)
//     {
//         var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
//         return Ok(orders);
//     }
//
//     // Get orders for a specific vendor
//     [HttpGet("vendor/{vendorEmail}")]
//     public async Task<IActionResult> GetOrdersByVendor(string vendorEmail)
//     {
//         var orders = await _orderService.GetOrdersByVendorAsync(vendorEmail);
//         return Ok(orders);
//     }
//
//     // Update the status of a product by a vendor
//     [HttpPut("update-status/{orderId}/{productId}")]
//     public async Task<IActionResult> UpdateProductStatus(string orderId, string productId, [FromQuery] string vendorEmail, [FromQuery] string status)
//     {
//         await _orderService.UpdateProductStatusAsync(orderId, productId, vendorEmail, status);
//         return Ok("Product status updated successfully.");
//     }
//
//     // Cancel an order by customer or admin
//     [HttpPut("cancel/{orderId}")]
//     public async Task<IActionResult> CancelOrder(string orderId, [FromBody] string note)
//     {
//         await _orderService.CancelOrderAsync(orderId, note);
//         return Ok("Order cancelled successfully.");
//     }
//
//     // Admin deletes an order
//     [HttpDelete("{orderId}")]
//     public async Task<IActionResult> DeleteOrder(string orderId)
//     {
//         await _orderService.DeleteOrderAsync(orderId);
//         return Ok("Order deleted successfully.");
//     }
// }


[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ProductService _productService;

    // Inject both services in the constructor
    public OrderController(OrderService orderService, ProductService productService)
    {
        _orderService = orderService;
        _productService = productService;
    }

    // Create new order
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDTO orderDto)
    {
        //var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var customerId = User.FindFirst("UserId")?.Value;

        // Prepare a list to hold the order items
        var orderItems = new List<OrderItem>();

        // Use a foreach loop to map OrderItemDTO to OrderItem asynchronously
        foreach (var itemDto in orderDto.Items)
        {
            var vendorEmail = await _productService.GetVendorEmailByProductIdAsync(itemDto.ProductId);  // Get the vendor email for each product

            var orderItem = new OrderItem
            {
                Id = ObjectId.GenerateNewId().ToString(), 
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                VendorEmail = vendorEmail
            };

            orderItems.Add(orderItem);
        }

        var order = new Order
        {
            CustomerId = customerId,
            Items = orderItems,
            CustomerNote = orderDto.CustomerNote
        };

        await _orderService.CreateOrderAsync(order);
        return Ok("Order created successfully.");
    }





    // Update order status
    [Authorize(Roles = "CSR, Admin")]
    [HttpPut("status/{orderId}")]
    public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] string status)
    {
        await _orderService.UpdateOrderStatusAsync(orderId, status);
        return Ok("Order status updated.");
    }

    // Cancel order
    [Authorize(Roles = "CSR, Admin")]
    [HttpPut("cancel/{orderId}")]
    public async Task<IActionResult> CancelOrder(string orderId, [FromBody] string cancellationNote)
    {
        await _orderService.CancelOrderAsync(orderId, cancellationNote);
        return Ok("Order canceled.");
    }

    // Mark order as delivered
    [Authorize(Roles = "CSR, Admin, Vendor")]
    [HttpPut("deliver/{orderId}")]
    public async Task<IActionResult> MarkOrderAsDelivered(string orderId, [FromQuery] string vendorEmail = null)
    {
        await _orderService.MarkOrderAsDelivered(orderId, vendorEmail);
        return Ok("Order marked as delivered.");
    }
    
    

    
    
    
    [Authorize(Roles = "Vendor, Admin")]
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        // Get the vendor's email from the JWT token
        var vendorEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        // If no vendor email found, return unauthorized
        if (string.IsNullOrEmpty(vendorEmail))
        {
            return Unauthorized("Vendor email not found in the token.");
        }

        // Call the service method to get orders by vendor email
        var orders = await _orderService.GetOrdersByVendorEmailAsync(vendorEmail);
    
        if (orders == null || !orders.Any())
        {
            return NotFound($"No orders found for vendor: {vendorEmail}");
        }

        return Ok(orders);
    }


}
