// using System.Security.Claims;
// using BackendServices.DTOs;
// using BackendServices.Services;
// using Microsoft.AspNetCore.Authorization;
//

using System.Security.Claims;
using BackendServices.DTOs;
using BackendServices.Helpers;
using BackendServices.Models;
using BackendServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendServices.Controllers
//
// // Controllers/ProductController.cs
// using Microsoft.AspNetCore.Mvc;
// using System.Threading.Tasks;
//
// [ApiController]
// [Route("api/[controller]")]
// public class ProductController : ControllerBase
// {
//     private readonly ProductService _productService;
//
//     public ProductController(ProductService productService)
//     {
//         _productService = productService;
//     }
//
//     [HttpGet]
//     public async Task<IActionResult> GetAllProducts()
//     {
//         var products = await _productService.GetAllProducts();
//         return Ok(products);
//     }
//
//     [HttpGet("{id}")]
//     public async Task<IActionResult> GetProductById(string id)
//     {
//         var product = await _productService.GetProductById(id);
//         if (product == null) return NotFound();
//         return Ok(product);
//     }
//
//     //[HttpPost]
//     //[Authorize(Roles = "Vendor")]
//     // public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productDTO)
//     // {
//     //     //var vendorId = User.Identity.Name; // Assuming authentication provides vendor ID
//     //     var vendorId = User.Claims.FirstOrDefault(c => c.Type == "VendorId")?.Value;
//     //     
//     //     if (string.IsNullOrEmpty(vendorId))
//     //     {
//     //         return Unauthorized("Vendor ID not found.");
//     //     }
//     //
//     //     await _productService.CreateProduct(productDTO, vendorId);
//     //     return CreatedAtAction(nameof(GetProductById), new { id = productDTO.Name }, productDTO);
//     // }
//     
//     [HttpPost]
//     [Authorize(Roles = "Vendor")]
//     public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productDTO)
//     {
//         // Check authentication state
//         if (!User.Identity.IsAuthenticated)
//         {
//             return Unauthorized("User is not authenticated.");
//         }
//
//         var vendorId = User.Claims.FirstOrDefault(c => c.Type == "VendorId")?.Value;
//
//         if (string.IsNullOrEmpty(vendorId))
//         {
//             return Unauthorized("Vendor ID not found.");
//         }
//
//         await _productService.CreateProduct(productDTO, vendorId);
//         return CreatedAtAction(nameof(GetProductById), new { id = productDTO.Name }, productDTO);
//     }
//
//
//
//     [HttpPut("{id}")]
//     public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductDTO productDTO)
//     {
//         await _productService.UpdateProduct(id, productDTO);
//         return NoContent();
//     }
//
//     [HttpDelete("{id}")]
//     public async Task<IActionResult> DeleteProduct(string id)
//     {
//         await _productService.DeleteProduct(id);
//         return NoContent();
//     }
//
//     [HttpPost("{id}/activate")]
//     public async Task<IActionResult> ActivateProduct(string id)
//     {
//         await _productService.ActivateProduct(id);
//         return NoContent();
//     }
//
//     [HttpPost("{id}/deactivate")]
//     public async Task<IActionResult> DeactivateProduct(string id)
//     {
//         await _productService.DeactivateProduct(id);
//         return NoContent();
//     }
// }


{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly JwtHelper _jwtHelper;

        public ProductController(ProductService productService, JwtHelper jwtHelper)
        {
            _productService = productService;
            _jwtHelper = jwtHelper;
        }

        // Get all products with vendor details (For all users)
        [Authorize]
        [HttpGet("listwithEmail")]
        public async Task<IActionResult> ListProductswithEmail()
        {
            var products = await _productService.GetProductsWithVendorEmailAsync();
            return Ok(products);
        }

        [Authorize]
        [HttpGet("list")]
        // public async Task<IActionResult> ListProducts()
        // {
        //     var products = await _productService.GetProductsWithVendorDetailsAsync();
        //     return Ok(products);
        // }


        // Vendor creates a product
        [Authorize(Roles = "Vendor")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productModel)
        {
            var vendorEmail = User.FindFirst(ClaimTypes.Email)?.Value;  // Vendor Email from token

            // Map ProductModel to Product entity
            var product = new Product
            {
                ProductName = productModel.ProductName,
                Price = productModel.Price,
                AvailableQuantity = productModel.AvailableQuantity,
                Category = productModel.Category,
                Description = productModel.Description,
                Image = productModel.Image,
                VendorEmail = vendorEmail,  // Store Vendor's Email instead of VendorId
                //StockStatus = 2,  // Default In Stock
                ProductStatus = false , // Default Active
                CreatedDate = productModel.CreatedDate,
            };

            await _productService.CreateProductAsync(product);
            return Ok("Product created successfully.");
        }

        // Vendor updates a product
        [Authorize(Roles = "Vendor")]
        [HttpPut("update/{productId}")]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] ProductDTO productUpdate)
        {
            var existingProduct = await _productService.GetProductByIdAsync(productId);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            var vendorEmail = User.FindFirst(ClaimTypes.Email)?.Value;  // Vendor Email from token
            if (existingProduct.VendorEmail != vendorEmail)
            {
                return Unauthorized("You can only update your own products.");
            }

            // Update fields
            existingProduct.ProductName = productUpdate.ProductName;
            existingProduct.Price = productUpdate.Price;
            existingProduct.AvailableQuantity = productUpdate.AvailableQuantity;
            existingProduct.Category = productUpdate.Category;
            existingProduct.Description = productUpdate.Description;
            existingProduct.Image = productUpdate.Image;

            // Vendor cannot change StockStatus or CategoryStatus
            await _productService.UpdateProductAsync(existingProduct);
            return Ok("Product updated successfully.");
        }

        // Vendor deletes a product
        [Authorize(Roles = "Vendor")]
        [HttpDelete("delete/{productId}")]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            var existingProduct = await _productService.GetProductByIdAsync(productId);
            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            var vendorEmail = User.FindFirst(ClaimTypes.Email)?.Value;  // Vendor Email from token
            if (existingProduct.VendorEmail != vendorEmail)
            {
                return Unauthorized("You can only delete your own products.");
            }

            await _productService.DeleteProductAsync(productId);
            return Ok("Product deleted successfully.");
        }

        // Admin updates stock status
        [Authorize(Roles = "Admin")]
        [HttpPut("stock-status/{productId}")]
        public async Task<IActionResult> UpdateStockStatus(string productId, [FromBody] int stockStatus)
        {
            await _productService.UpdateStockStatusAsync(productId, stockStatus);
            return Ok("Stock status updated successfully.");
        }

        // Admin updates category status
        // [Authorize(Roles = "Admin")]
        // [HttpPut("product-status")]
        // public async Task<IActionResult> UpdateCategoryStatus([FromBody] CategoryStatusModel categoryStatusModel)
        // {
        //     await _productService.UpdateCategoryStatusAsync(categoryStatusModel.Category, categoryStatusModel.CategoryStatus);
        //     return Ok("Category status updated successfully.");
        // }
        
        
        [Authorize(Roles = "Admin")]
        [HttpPut("product-status/{productId}")]
        public async Task<IActionResult> UpdateProductStatus(string productId, [FromBody] bool productStatus)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            await _productService.UpdateProductStatusAsync(productId, productStatus);
            return Ok("Product status updated successfully.");
        }
        
        
        [Authorize(Roles = "Admin")]
        [HttpGet("products-by-status")]
        public async Task<IActionResult> GetProductsByStatus([FromQuery] string category, [FromQuery] bool? productStatus = null, [FromQuery] DateTime? startDate = null)
        {
            // Get the list of products filtered by ProductStatus, category, and date
            var products = await _productService.GetProductsByStatusAsync(category, productStatus, startDate);
            return Ok(products);
        }
        
        
        [Authorize]
        [HttpGet("category/{categoryName}")]
        public async Task<IActionResult> GetProductsByCategory(string categoryName)
        {
            // Call the service method to get products by category
            var products = await _productService.GetProductsByCategoryAsync(categoryName);
    
            if (products == null || !products.Any())
            {
                return NotFound($"No products found in the category: {categoryName}");
            }
    
            return Ok(products);
        }





        
        

        // Fetch specific product details
        [Authorize]
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProduct(string productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            return Ok(product);
        }
    }
    
    
    
    
    
    // public class CategoryStatusModel
    // {
    //     public string Category { get; set; }
    //     public int CategoryStatus { get; set; }
    // }
}
