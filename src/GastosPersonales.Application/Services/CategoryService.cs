using GastosPersonales.Application.DTOs.Category;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;

namespace GastosPersonales.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllByUserIdAsync(Guid userId)
    {
        var categories = await _categoryRepository.GetAllByUserIdAsync(userId);
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        });
    }

    public async Task<CategoryDto> GetByIdAsync(Guid id, Guid userId)
    {
        var category = await _categoryRepository.GetByIdAsync(id, userId);
        if (category == null)
        {
            throw new KeyNotFoundException("Categoría no encontrada.");
        }

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }

    public async Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto dto)
    {
        if (await _categoryRepository.ExistsByNameAsync(dto.Name, userId))
        {
            throw new InvalidOperationException("Ya existe una categoría con ese nombre.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        category = await _categoryRepository.CreateAsync(category);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, Guid userId, UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id, userId);
        if (category == null)
        {
            throw new KeyNotFoundException("Categoría no encontrada.");
        }

        if (await _categoryRepository.ExistsByNameAsync(dto.Name, userId, id))
        {
            throw new InvalidOperationException("Ya existe una categoría con ese nombre.");
        }

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        category = await _categoryRepository.UpdateAsync(category);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var category = await _categoryRepository.GetByIdAsync(id, userId);
        if (category == null)
        {
            throw new KeyNotFoundException("Categoría no encontrada.");
        }

        if (await _categoryRepository.HasExpensesAsync(id))
        {
            throw new InvalidOperationException("No se puede eliminar una categoría que tiene gastos asociados.");
        }

        await _categoryRepository.DeleteAsync(id, userId);
    }
}

