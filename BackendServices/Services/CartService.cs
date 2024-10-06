using System.Net;
using System.Net.Mail;
using BackendServices.DTOs;
using BackendServices.Models;
using MongoDB.Driver;

namespace BackendServices.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly EmailService _emailService;

        public CartService(ICartRepository cartRepository, IMongoDatabase database)
        {
            _cartRepository = cartRepository;
            _productCollection = database.GetCollection<Product>("Products"); // Accessing product collection
        }

        // Add or update an item in the cart
        public async Task<Cart> AddOrUpdateCartItemAsync(string customerId, CartItemDto cartItemDto)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId) ?? new Cart { CustomerId = customerId };

            // Fetch the product details from the product collection
            var product = await _productCollection.Find(p => p.Id == cartItemDto.ProductId).FirstOrDefaultAsync();
            Console.WriteLine($"Fetching product with ID: {cartItemDto.ProductId}");
            Console.WriteLine($"User Id add item: {customerId}");

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            // Check stock availability
            if (cartItemDto.Quantity > product.AvailableQuantity)
            {
                throw new Exception("Insufficient stock.");
            }

            // Check if the item already exists in the cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == cartItemDto.ProductId);
            if (existingItem != null)
            {
                // Update the quantity
                existingItem.Quantity += cartItemDto.Quantity;

                // Check if the updated quantity exceeds the available stock
                if (existingItem.Quantity > product.AvailableQuantity)
                {
                    throw new Exception("Insufficient stock.");
                }
            }
            else
            {
                // Add new item
                cart.Items.Add(new CartItem
                {
                    ProductId = cartItemDto.ProductId,
                    Quantity = cartItemDto.Quantity,
                    Price = product.Price  // Use the product price fetched from the product collection
                });
            }

            return await _cartRepository.UpdateCartAsync(cart);
        }

        // Remove an item from the cart
        public async Task<bool> RemoveCartItemAsync(string customerId, string productId)
        {
            return await _cartRepository.RemoveCartItemAsync(customerId, productId);
        }

        // Get the cart by customer ID and include product details
        public async Task<CartWithProductDetailsDto> GetCartWithDetailsByCustomerIdAsync(string customerId)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
            
            // if (cart == null) return null;

            var cartWithDetails = new CartWithProductDetailsDto
            {
                CustomerId = cart.CustomerId,
                Items = new List<CartItemWithDetailsDto>(),
                TotalPrice = 0
            };

            foreach (var item in cart.Items)
            {
                // Fetch the product details for each item
                var product = await _productCollection.Find(p => p.Id == item.ProductId).FirstOrDefaultAsync();
                if (product == null) continue;

                cartWithDetails.Items.Add(new CartItemWithDetailsDto
                {
                    ProductId = item.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    AvailableQuantity = product.AvailableQuantity,
                    Category = product.Category,
                    Description = product.Description,
                    Image = product.Image
                });
                
                var client = new SmtpClient("bulk.smtp.mailtrap.io", 587)
                {
                    Credentials = new NetworkCredential("smtp@mailtrap.io", "2c60006e49265889677f41b8d86cff98"),
                    EnableSsl = true
                };
                client.Send("hello@demomailtrap.com", "tharuneo37@gmail.com", "Adoooo", "Wede Goda yakoo");
                System.Console.WriteLine("Sent");            
            }

            return cartWithDetails;
        }
    }
}
