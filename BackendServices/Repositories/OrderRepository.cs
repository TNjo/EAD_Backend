using BackendServices.Configurations;
using BackendServices.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BackendServices;

// public class OrderRepository : IOrderRepository
// {
//     private readonly IMongoCollection<Order> _orderCollection;
//
//     public OrderRepository(IMongoDatabase database)
//     {
//         _orderCollection = database.GetCollection<Order>("orders");
//     }
//
//     public async Task CreateOrderAsync(Order order)
//     {
//         await _orderCollection.InsertOneAsync(order);
//     }
//
//     public async Task<Order> GetOrderByIdAsync(string orderId)
//     {
//         return await _orderCollection.Find(o => o.Id == orderId).FirstOrDefaultAsync();
//     }
//
//     public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
//     {
//         return await _orderCollection.Find(o => o.CustomerId == customerId).ToListAsync();
//     }
//
//     public async Task<List<Order>> GetOrdersByVendorEmailAsync(string vendorEmail)
//     {
//         return await _orderCollection.Find(o => o.Items.Any(i => i.VendorEmail == vendorEmail)).ToListAsync();
//     }
//
//     public async Task UpdateOrderAsync(Order order)
//     {
//         await _orderCollection.ReplaceOneAsync(o => o.Id == order.Id, order);
//     }
//
//     public async Task DeleteOrderAsync(string orderId)
//     {
//         await _orderCollection.DeleteOneAsync(o => o.Id == orderId);
//     }
// }


public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders;

    public OrderRepository(IOptions<MongoDBSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _orders = database.GetCollection<Order>("Orders");
        
    }

    public async Task CreateOrderAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task<Order> GetOrderByIdAsync(string orderId)
    {
        return await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
    }

    public async Task UpdateOrderAsync(Order order)
    {
        await _orders.ReplaceOneAsync(o => o.Id == order.Id, order);
    }
    
    
    // Fetch orders by vendor email
    public async Task<List<Order>> GetOrdersByVendorEmailAsync(string vendorEmail)
    {
        var filter = Builders<Order>.Filter.ElemMatch(o => o.Items, item => item.VendorEmail == vendorEmail);
        return await _orders.Find(filter).ToListAsync();
    }
    
 
}
