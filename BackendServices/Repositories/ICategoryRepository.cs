using BackendServices.Models;

namespace BackendServices;

public interface ICategoryRepository
{
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(string id, Category category);
    Task<Category> GetCategoryByIdAsync(string id);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<bool> DeleteCategoryAsync(string id);
}