using GastosPersonales.Application.DTOs.Budget;

namespace GastosPersonales.Application.Services;

public interface IBudgetService
{
    Task<IEnumerable<BudgetDto>> GetAllByUserIdAsync(Guid userId);
    Task<IEnumerable<BudgetDto>> GetByMonthAsync(int month, int year, Guid userId);
    Task<BudgetDto> GetByIdAsync(Guid id, Guid userId);
    Task<BudgetDto> CreateAsync(Guid userId, CreateBudgetDto dto);
    Task<BudgetDto> UpdateAsync(Guid id, Guid userId, UpdateBudgetDto dto);
    Task DeleteAsync(Guid id, Guid userId);
}

