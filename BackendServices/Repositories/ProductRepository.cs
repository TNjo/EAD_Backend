// using BackendServices.Models;
//
// namespace BackendServices;
//
// // Repositories/ProductRepository.cs
// using MongoDB.Driver;
// using System.Collections.Generic;
// using System.Threading.Tasks;
//
// public class ProductRepository : IProductRepository
// {
//     private readonly IMongoCollection<Product> _products;
//
//     public ProductRepository(IMongoDatabase mongoDatabase)
//     {
//         _products = mongoDatabase.GetCollection<Product>("Products");
//     }
//
//     public async Task<IEnumerable<Product>> GetAllProducts() => await _products.Find(p => true).ToListAsync();
//
//     public async Task<Product> GetProductById(string productId) => 
//         await _products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
//
//     public async Task CreateProduct(Product product) => await _products.InsertOneAsync(product);
//
//     public async Task UpdateProduct(Product product) =>
//         await _products.ReplaceOneAsync(p => p.ProductId == product.ProductId, product);
//
//     public async Task DeleteProduct(string productId) =>
//         await _products.DeleteOneAsync(p => p.ProductId == productId);
//
//     public async Task ActivateProduct(string productId)
//     {
//         var product = await GetProductById(productId);
//         if (product != null)
//         {
//             product.IsActive = true;
//             await UpdateProduct(product);
//         }
//     }
//
//     public async Task DeactivateProduct(string productId)
//     {
//         var product = await GetProductById(productId);
//         if (product != null)
//         {
//             product.IsActive = false;
//             await UpdateProduct(product);
//         }
//     }
// }


using BackendServices.Configurations;
using BackendServices.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(IOptions<MongoDBSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _products = database.GetCollection<Product>("Products");
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await _products.Find(product => true).ToListAsync();
    }

    public async Task<Product> GetProductByIdAsync(string productId)
    {
        return await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
    }

    public async Task CreateProductAsync(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
    }

    public async Task DeleteProductAsync(string productId)
    {
        await _products.DeleteOneAsync(p => p.Id == productId);
    }

    public async Task UpdateStockStatusAsync(string productId, int stockStatus)
    {
        var product = await GetProductByIdAsync(productId);
        if (product != null)
        {
            product.StockStatus = stockStatus;
            await UpdateProductAsync(product);
        }
    }

    // public async Task UpdateCategoryStatusAsync(string category, int categoryStatus)
    // {
    //     var filter = Builders<Product>.Filter.Eq("Category", category);
    //     var update = Builders<Product>.Update.Set("CategoryStatus", categoryStatus);
    //     await _products.UpdateManyAsync(filter, update);
    // }
    
    public async Task UpdateProductStatusAsync(string productId, bool productStatus)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, productId);
        var update = Builders<Product>.Update.Set(p => p.ProductStatus, productStatus);
        await _products.UpdateOneAsync(filter, update);
    }
    
    
    // Fetch products filtered by ProductStatus, category, and date
    public async Task<List<Product>> GetProductsByStatusAsync(string category, bool? productStatus, DateTime? startDate)
    {
        var filterBuilder = Builders<Product>.Filter;

        // Filter by category
        var filter = filterBuilder.Eq(p => p.Category, category);

        // Filter by ProductStatus if it's provided
        if (productStatus.HasValue)
        {
            filter = filter & filterBuilder.Eq(p => p.ProductStatus, productStatus.Value);
        }

        // Filter by date if startDate is provided
        if (startDate.HasValue)
        {
            filter = filter & filterBuilder.Gte(p => p.CreatedDate, startDate.Value);
        }

        return await _products.Find(filter).ToListAsync();
    }
    
    
    // Fetch products filtered by category
    public async Task<List<Product>> GetProductsByCategoryAsync(string categoryName)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);
        return await _products.Find(filter).ToListAsync();
    }
    

}
