using BackendServices.Models;

namespace BackendServices;

public interface ICartRepository
{
    Task<Cart> GetCartByCustomerIdAsync(string customerId);
    Task<Cart> UpdateCartAsync(Cart cart);
    Task<bool> RemoveCartItemAsync(string customerId, string productId);
}
