using GastosPersonales.Domain.Entities;

namespace GastosPersonales.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<Category>> GetAllByUserIdAsync(Guid userId);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<bool> ExistsByNameAsync(string name, Guid userId, Guid? excludeId = null);
    Task<bool> HasExpensesAsync(Guid id);
}

