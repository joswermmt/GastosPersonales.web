using GastosPersonales.Application.DTOs.Budget;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;

namespace GastosPersonales.Application.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IExpenseRepository _expenseRepository;

    public BudgetService(
        IBudgetRepository budgetRepository,
        ICategoryRepository categoryRepository,
        IExpenseRepository expenseRepository)
    {
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<IEnumerable<BudgetDto>> GetAllByUserIdAsync(Guid userId)
    {
        var budgets = await _budgetRepository.GetAllByUserIdAsync(userId);
        var result = new List<BudgetDto>();

        foreach (var budget in budgets)
        {
            var spent = await _expenseRepository.GetTotalByCategoryAndMonthAsync(
                budget.CategoryId, budget.Month, budget.Year);

            result.Add(MapToDto(budget, spent));
        }

        return result;
    }

    public async Task<IEnumerable<BudgetDto>> GetByMonthAsync(int month, int year, Guid userId)
    {
        var budgets = await _budgetRepository.GetByMonthAsync(month, year, userId);
        var result = new List<BudgetDto>();

        foreach (var budget in budgets)
        {
            var spent = await _expenseRepository.GetTotalByCategoryAndMonthAsync(
                budget.CategoryId, budget.Month, budget.Year);

            result.Add(MapToDto(budget, spent));
        }

        return result;
    }

    public async Task<BudgetDto> GetByIdAsync(Guid id, Guid userId)
    {
        var budget = await _budgetRepository.GetByIdAsync(id, userId);
        if (budget == null)
        {
            throw new KeyNotFoundException("Presupuesto no encontrado.");
        }

        var spent = await _expenseRepository.GetTotalByCategoryAndMonthAsync(
            budget.CategoryId, budget.Month, budget.Year);

        return MapToDto(budget, spent);
    }

    public async Task<BudgetDto> CreateAsync(Guid userId, CreateBudgetDto dto)
    {
        if (dto.Amount <= 0)
        {
            throw new ArgumentException("El monto del presupuesto debe ser mayor a cero.");
        }

        if (dto.Month < 1 || dto.Month > 12)
        {
            throw new ArgumentException("El mes debe estar entre 1 y 12.");
        }

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId);
        if (category == null)
        {
            throw new KeyNotFoundException("Categoría no encontrada.");
        }

        var existing = await _budgetRepository.GetByCategoryAndMonthAsync(
            dto.CategoryId, dto.Month, dto.Year, userId);
        if (existing != null)
        {
            throw new InvalidOperationException("Ya existe un presupuesto para esta categoría en el mes especificado.");
        }

        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Month = dto.Month,
            Year = dto.Year,
            CreatedAt = DateTime.UtcNow
        };

        budget = await _budgetRepository.CreateAsync(budget);
        budget.Category = category;

        var spent = await _expenseRepository.GetTotalByCategoryAndMonthAsync(
            budget.CategoryId, budget.Month, budget.Year);

        return MapToDto(budget, spent);
    }

    public async Task<BudgetDto> UpdateAsync(Guid id, Guid userId, UpdateBudgetDto dto)
    {
        if (dto.Amount <= 0)
        {
            throw new ArgumentException("El monto del presupuesto debe ser mayor a cero.");
        }

        var budget = await _budgetRepository.GetByIdAsync(id, userId);
        if (budget == null)
        {
            throw new KeyNotFoundException("Presupuesto no encontrado.");
        }

        budget.Amount = dto.Amount;
        budget.UpdatedAt = DateTime.UtcNow;

        budget = await _budgetRepository.UpdateAsync(budget);

        var spent = await _expenseRepository.GetTotalByCategoryAndMonthAsync(
            budget.CategoryId, budget.Month, budget.Year);

        return MapToDto(budget, spent);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var budget = await _budgetRepository.GetByIdAsync(id, userId);
        if (budget == null)
        {
            throw new KeyNotFoundException("Presupuesto no encontrado.");
        }

        await _budgetRepository.DeleteAsync(id, userId);
    }

    private static BudgetDto MapToDto(Budget budget, decimal spent)
    {
        var remaining = budget.Amount - spent;
        var percentage = budget.Amount > 0 ? (double)(spent / budget.Amount * 100) : 0;

        string alertLevel;
        if (percentage >= 100)
            alertLevel = "Excedido";
        else if (percentage >= 80)
            alertLevel = "Alto";
        else if (percentage >= 50)
            alertLevel = "Medio";
        else
            alertLevel = "Bajo";

        return new BudgetDto
        {
            Id = budget.Id,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category.Name,
            Amount = budget.Amount,
            SpentAmount = spent,
            RemainingAmount = remaining,
            PercentageUsed = percentage,
            AlertLevel = alertLevel,
            Month = budget.Month,
            Year = budget.Year
        };
    }
}

