using GastosPersonales.Application.DTOs.Expense;

namespace GastosPersonales.Application.Services;

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDto>> GetAllByUserIdAsync(Guid userId);
    Task<IEnumerable<ExpenseDto>> GetFilteredAsync(Guid userId, ExpenseFilterDto filter);
    Task<ExpenseDto> GetByIdAsync(Guid id, Guid userId);
    Task<ExpenseDto> CreateAsync(Guid userId, CreateExpenseDto dto);
    Task<ExpenseDto> UpdateAsync(Guid id, Guid userId, UpdateExpenseDto dto);
    Task DeleteAsync(Guid id, Guid userId);
}

