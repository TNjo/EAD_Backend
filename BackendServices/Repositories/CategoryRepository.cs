using BackendServices.Models;
using MongoDB.Driver;

namespace BackendServices;

public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<Category> _categoryCollection;

    public CategoryRepository(IMongoDatabase database)
    {
        _categoryCollection = database.GetCollection<Category>("categories");
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        await _categoryCollection.InsertOneAsync(category);
        return category; // Return the created category
    }

    public async Task<Category> UpdateCategoryAsync(string id, Category category)
    {
        await _categoryCollection.ReplaceOneAsync(c => c.Id == id, category);
        return category; // Return the updated category
    }

    public async Task<Category> GetCategoryByIdAsync(string id)
    {
        return await _categoryCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryCollection.Find(_ => true).ToListAsync();
    }

    public async Task<bool> DeleteCategoryAsync(string id)
    {
        var result = await _categoryCollection.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0; // Return true if the category was deleted
    }
}