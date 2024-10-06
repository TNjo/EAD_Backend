using BackendServices.DTOs;
using BackendServices.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    // Helper method to get the User ID from the JWT token
    private string GetUserIdFromToken()
    {
        var userId = User.FindFirst("UserId")?.Value;
        if (userId == null)
        {
            throw new UnauthorizedAccessException("User not authenticated.");
        }
        return userId;
    }

    // Get the cart for the currently authenticated user with product details
    [HttpGet]
    public async Task<IActionResult> GetCartByCustomerId()
    {
        try
        {
            var userId = GetUserIdFromToken(); // Get the logged-in user ID
            var cartWithDetails = await _cartService.GetCartWithDetailsByCustomerIdAsync(userId);
            if (cartWithDetails == null) return NotFound();
            return Ok(cartWithDetails);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    // Add or update an item in the cart for the currently authenticated user
    [HttpPost("add-item")]
    public async Task<IActionResult> AddOrUpdateCartItem([FromBody] CartItemDto cartItemDto)
    {
        try
        {
            var userId = GetUserIdFromToken(); // Get the logged-in user ID
            var cart = await _cartService.AddOrUpdateCartItemAsync(userId, cartItemDto);
            return Ok(cart);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // Handle any other exceptions (like insufficient stock)
        }
    }

    // Remove an item from the cart for the currently authenticated user
    [HttpDelete("remove-item/{productId}")]
    public async Task<IActionResult> RemoveCartItem(string productId)
    {
        try
        {
            var userId = GetUserIdFromToken(); // Get the logged-in user ID
            var result = await _cartService.RemoveCartItemAsync(userId, productId);
            if (!result) return NotFound("Item not found in cart.");
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
