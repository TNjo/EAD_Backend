using BackendServices.Models;
using MongoDB.Driver;

namespace BackendServices;

public class CartRepository : ICartRepository
{
    private readonly IMongoCollection<Cart> _cartCollection;

    public CartRepository(IMongoDatabase database)
    {
        _cartCollection = database.GetCollection<Cart>("carts");
    }

    // Get the cart by customer ID
    public async Task<Cart> GetCartByCustomerIdAsync(string customerId)
    {
        Console.WriteLine(await _cartCollection.Find(cart => cart.CustomerId == customerId).FirstOrDefaultAsync());
        return await _cartCollection.Find(cart => cart.CustomerId == customerId).FirstOrDefaultAsync();

    }

    // Update the cart (upsert)
    public async Task<Cart> UpdateCartAsync(Cart cart)
    {
        var options = new ReplaceOptions { IsUpsert = true }; // Upsert: Insert if not exists, else update
        await _cartCollection.ReplaceOneAsync(c => c.CustomerId == cart.CustomerId, cart, options);
        return cart;
    }

    // Delete an item from the cart
    public async Task<bool> RemoveCartItemAsync(string customerId, string productId)
    {
        var update = Builders<Cart>.Update.PullFilter(cart => cart.Items, item => item.ProductId == productId);
        var result = await _cartCollection.UpdateOneAsync(cart => cart.CustomerId == customerId, update);
        return result.ModifiedCount > 0;
    }
}
