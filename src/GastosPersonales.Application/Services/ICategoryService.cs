using GastosPersonales.Application.DTOs.Category;

namespace GastosPersonales.Application.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllByUserIdAsync(Guid userId);
    Task<CategoryDto> GetByIdAsync(Guid id, Guid userId);
    Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto dto);
    Task<CategoryDto> UpdateAsync(Guid id, Guid userId, UpdateCategoryDto dto);
    Task DeleteAsync(Guid id, Guid userId);
}

