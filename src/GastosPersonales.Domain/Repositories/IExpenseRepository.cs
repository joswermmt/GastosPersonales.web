using GastosPersonales.Domain.Entities;

namespace GastosPersonales.Domain.Repositories;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<Expense>> GetAllByUserIdAsync(Guid userId);
    Task<IEnumerable<Expense>> GetFilteredAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? categoryId = null,
        Guid? paymentMethodId = null,
        string? searchText = null
    );
    Task<Expense> CreateAsync(Expense expense);
    Task<Expense> UpdateAsync(Expense expense);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<decimal> GetTotalByCategoryAndMonthAsync(Guid categoryId, int month, int year);
    Task<IEnumerable<Expense>> GetByCategoryAndMonthAsync(Guid categoryId, int month, int year);
}

