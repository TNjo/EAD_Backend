using BackendServices.DTOs;
using BackendServices.Models;

namespace BackendServices.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Category> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name
        };

        return await _categoryRepository.CreateCategoryAsync(category);
    }

    public async Task<Category> UpdateCategoryAsync(string id, CategoryDto categoryDto)
    {
        var category = new Category
        {
            Id = id,
            Name = categoryDto.Name
        };

        return await _categoryRepository.UpdateCategoryAsync(id, category);
    }

    public async Task<Category> GetCategoryByIdAsync(string id)
    {
        return await _categoryRepository.GetCategoryByIdAsync(id);
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllCategoriesAsync();
    }

    public async Task<bool> DeleteCategoryAsync(string id)
    {
        return await _categoryRepository.DeleteCategoryAsync(id);
    }
}