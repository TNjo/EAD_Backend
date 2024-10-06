// using BackendServices.DTOs;
// using BackendServices.Models;
//
// namespace BackendServices.Services;
//
// // Services/ProductService.cs
// using System.Collections.Generic;
// using System.Threading.Tasks;
//
// public class ProductService
// {
//     private readonly IProductRepository _productRepository;
//
//     public ProductService(IProductRepository productRepository)
//     {
//         _productRepository = productRepository;
//     }
//
//     public async Task<IEnumerable<Product>> GetAllProducts() => await _productRepository.GetAllProducts();
//
//     public async Task<Product> GetProductById(string productId) => await _productRepository.GetProductById(productId);
//
//     public async Task CreateProduct(ProductDTO productDTO, string vendorId)
//     {
//         var product = new Product
//         {
//             Name = productDTO.Name,
//             Description = productDTO.Description,
//             Price = productDTO.Price,
//             VendorId = vendorId,
//             IsActive = true
//         };
//         await _productRepository.CreateProduct(product);
//     }
//
//     public async Task UpdateProduct(string productId, ProductDTO productDTO)
//     {
//         var product = await _productRepository.GetProductById(productId);
//         if (product != null)
//         {
//             product.Name = productDTO.Name;
//             product.Description = productDTO.Description;
//             product.Price = productDTO.Price;
//             await _productRepository.UpdateProduct(product);
//         }
//     }
//
//     public async Task DeleteProduct(string productId) => await _productRepository.DeleteProduct(productId);
//
//     public async Task ActivateProduct(string productId) => await _productRepository.ActivateProduct(productId);
//
//     public async Task DeactivateProduct(string productId) => await _productRepository.DeactivateProduct(productId);
// }


using BackendServices.Configurations;
using BackendServices.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BackendServices.Services
{
    // public class ProductService
    // {
    //     private readonly IMongoCollection<Product> _products;
    //     private readonly VendorService _vendorService;
    //
    //     public ProductService(IOptions<MongoDBSettings> mongoSettings, VendorService vendorService)
    //     {
    //         var client = new MongoClient(mongoSettings.Value.ConnectionString);
    //         var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
    //         _products = database.GetCollection<Product>("Products");
    //         _vendorService = vendorService;
    //     }
    //
    //     public async Task<List<Product>> GetProductsAsync()
    //     {
    //         return await _products.Find(product => true).ToListAsync();
    //     }
    //
    //     public async Task<Product> GetProductByIdAsync(string productId)
    //     {
    //         return await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
    //     }
    //
    //     public async Task CreateProductAsync(Product product)
    //     {
    //         await _products.InsertOneAsync(product);
    //     }
    //
    //     public async Task UpdateProductAsync(Product product)
    //     {
    //         await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
    //     }
    //
    //     public async Task DeleteProductAsync(string productId)
    //     {
    //         await _products.DeleteOneAsync(p => p.Id == productId);
    //     }
    //
    //     // Change stock status by admin
    //     public async Task UpdateStockStatusAsync(string productId, int stockStatus)
    //     {
    //         var product = await GetProductByIdAsync(productId);
    //         if (product != null)
    //         {
    //             product.StockStatus = stockStatus;
    //             await UpdateProductAsync(product);
    //         }
    //     }
    //
    //     // Change category status by admin
    //     public async Task UpdateCategoryStatusAsync(string category, int categoryStatus)
    //     {
    //         var filter = Builders<Product>.Filter.Eq("Category", category);
    //         var update = Builders<Product>.Update.Set("CategoryStatus", categoryStatus);
    //         await _products.UpdateManyAsync(filter, update);
    //     }
    //
    //     
    //     // Fetch products including Vendor details
    //     public async Task<List<Product>> GetProductsWithVendorEmailAsync()
    //     {
    //         var products = await GetProductsAsync();
    //         foreach (var product in products)
    //         {
    //             // Fetch vendor details using the VendorEmail
    //             var vendor = await _vendorService.GetVendorByEmailAsync(product.VendorEmail);
    //             if (vendor != null)
    //             {
    //                 
    //                 product.VendorEmail = vendor.Email;
    //                
    //             }
    //         }
    //         return products;
    //     }
    //     // public async Task<List<ProductWithVendorDetails>> GetProductsWithVendorDetailsAsync()
    //     // {
    //     //     var products = await GetProductsAsync();
    //     //     var productWithVendorDetailsList = new List<ProductWithVendorDetails>();
    //     //
    //     //     foreach (var product in products)
    //     //     {
    //     //         // Fetch vendor details using the VendorEmail
    //     //         var vendor = await _vendorService.GetVendorByEmailAsync(product.VendorEmail);
    //     //         if (vendor != null)
    //     //         {
    //     //             // Create a new product with both vendorEmail and vendorId
    //     //             var productWithVendorDetails = new ProductWithVendorDetails
    //     //             {
    //     //                 Id = product.Id,
    //     //                 ProductName = product.ProductName,
    //     //                 Price = product.Price,
    //     //                 AvailableQuantity = product.AvailableQuantity,
    //     //                 Category = product.Category,
    //     //                 Description = product.Description,
    //     //                 Image = product.Image,
    //     //                 StockStatus = product.StockStatus,
    //     //                 CategoryStatus = product.CategoryStatus,
    //     //                 VendorEmail = product.VendorEmail,  // As stored in the product document
    //     //                 VendorId = vendor.Id  // VendorId fetched from the Vendor collection
    //     //             };
    //     //
    //     //             productWithVendorDetailsList.Add(productWithVendorDetails);
    //     //         }
    //     //     }
    //     //
    //     //     return productWithVendorDetailsList;
    //     // }
    //
    // }
    
    
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly VendorService _vendorService;

        public ProductService(IProductRepository productRepository, VendorService vendorService)
        {
            _productRepository = productRepository;
            _vendorService = vendorService;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _productRepository.GetProductsAsync();
        }

        public async Task<Product> GetProductByIdAsync(string productId)
        {
            return await _productRepository.GetProductByIdAsync(productId);
        }

        public async Task CreateProductAsync(Product product)
        {
            await _productRepository.CreateProductAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(string productId)
        {
            await _productRepository.DeleteProductAsync(productId);
        }

        // Change stock status by admin
        public async Task UpdateStockStatusAsync(string productId, int stockStatus)
        {
            await _productRepository.UpdateStockStatusAsync(productId, stockStatus);
        }

        // Change category status by admin
        // public async Task UpdateCategoryStatusAsync(string category, int categoryStatus)
        // {
        //     await _productRepository.UpdateCategoryStatusAsync(category, categoryStatus);
        // }

        
        public async Task UpdateProductStatusAsync(string productId, bool productStatus)
        {
            await _productRepository.UpdateProductStatusAsync(productId, productStatus);
        }

        
        // Fetch products including Vendor details
        public async Task<List<Product>> GetProductsWithVendorEmailAsync()
        {
            var products = await GetProductsAsync();
            foreach (var product in products)
            {
                // Fetch vendor details using the VendorEmail
                var vendor = await _vendorService.GetVendorByEmailAsync(product.VendorEmail);
                if (vendor != null)
                {
                    product.VendorEmail = vendor.Email;
                }
            }
            return products;
        }
        
        
        public async Task<string> GetVendorEmailByProductIdAsync(string productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found.");
            }

            return product.VendorEmail;
        }
        
        /// Fetch the products filtered by status, category, and optional date
        public async Task<List<Product>> GetProductsByStatusAsync(string category, bool? productStatus, DateTime? startDate)
        {
            return await _productRepository.GetProductsByStatusAsync(category, productStatus, startDate);
        }
        
        
        // Fetch the products by category
        public async Task<List<Product>> GetProductsByCategoryAsync(string categoryName)
        {
            return await _productRepository.GetProductsByCategoryAsync(categoryName);
        }
    }
}