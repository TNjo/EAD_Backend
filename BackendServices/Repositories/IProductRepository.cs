// using BackendServices.Models;
//
// namespace BackendServices;
//
// // Repositories/IProductRepository.cs
// using System.Collections.Generic;
// using System.Threading.Tasks;
//
// public interface IProductRepository
// {
//     Task<IEnumerable<Product>> GetAllProducts();
//     Task<Product> GetProductById(string productId);
//     Task CreateProduct(Product product);
//     Task UpdateProduct(Product product);
//     Task DeleteProduct(string productId);
//     Task ActivateProduct(string productId);
//     Task DeactivateProduct(string productId);
// }


using BackendServices.Models;

public interface IProductRepository
{
    Task<List<Product>> GetProductsAsync();
    Task<Product> GetProductByIdAsync(string productId);
    Task CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(string productId);
    Task UpdateStockStatusAsync(string productId, int stockStatus);
    //Task UpdateCategoryStatusAsync(string category, int categoryStatus);
    Task UpdateProductStatusAsync(string productId, bool productStatus);
    Task<List<Product>> GetProductsByStatusAsync(string category, bool? productStatus, DateTime? startDate);
    Task<List<Product>> GetProductsByCategoryAsync(string categoryName);
}