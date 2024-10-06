using BackendServices.DTOs;
using BackendServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // Create a new category
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
    {
        var category = await _categoryService.CreateCategoryAsync(categoryDto);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    // Update an existing category
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(string id, [FromBody] CategoryDto categoryDto)
    {
        var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDto);
        if (updatedCategory == null) return NotFound();
        return Ok(updatedCategory);
    }

    // Get a category by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(string id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    // Get all categories
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    // Delete a category
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}