using GastosPersonales.Domain.Entities;

namespace GastosPersonales.Domain.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id, Guid userId);
    Task<Budget?> GetByCategoryAndMonthAsync(Guid categoryId, int month, int year, Guid userId);
    Task<IEnumerable<Budget>> GetAllByUserIdAsync(Guid userId);
    Task<IEnumerable<Budget>> GetByMonthAsync(int month, int year, Guid userId);
    Task<Budget> CreateAsync(Budget budget);
    Task<Budget> UpdateAsync(Budget budget);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}

