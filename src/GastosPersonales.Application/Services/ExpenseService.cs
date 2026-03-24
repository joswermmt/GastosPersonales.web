using GastosPersonales.Application.DTOs.Expense;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;

namespace GastosPersonales.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public ExpenseService(
        IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository,
        IPaymentMethodRepository paymentMethodRepository)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<IEnumerable<ExpenseDto>> GetAllByUserIdAsync(Guid userId)
    {
        var expenses = await _expenseRepository.GetAllByUserIdAsync(userId);
        return expenses.Select(e => MapToDto(e));
    }

    public async Task<IEnumerable<ExpenseDto>> GetFilteredAsync(Guid userId, ExpenseFilterDto filter)
    {
        var expenses = await _expenseRepository.GetFilteredAsync(
            userId,
            filter.StartDate,
            filter.EndDate,
            filter.CategoryId,
            filter.PaymentMethodId,
            filter.SearchText);

        return expenses.Select(e => MapToDto(e));
    }

    public async Task<ExpenseDto> GetByIdAsync(Guid id, Guid userId)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, userId);
        if (expense == null)
        {
            throw new KeyNotFoundException("Gasto no encontrado.");
        }

        return MapToDto(expense);
    }

    public async Task<ExpenseDto> CreateAsync(Guid userId, CreateExpenseDto dto)
    {
        if (dto.Amount <= 0)
        {
            throw new ArgumentException("El monto debe ser mayor a cero.");
        }

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId);
        if (category == null)
        {
            throw new KeyNotFoundException("Categoría no encontrada.");
        }

        if (!category.IsActive)
        {
            throw new InvalidOperationException("La categoría no está activa.");
        }

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(dto.PaymentMethodId, userId);
        if (paymentMethod == null)
        {
            throw new KeyNotFoundException("Método de pago no encontrado.");
        }

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = dto.CategoryId,
            PaymentMethodId = dto.PaymentMethodId,
            Amount = dto.Amount,
            Date = dto.Date,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        expense = await _expenseRepository.CreateAsync(expense);

        // Load navigation properties
        expense.Category = category;
        expense.PaymentMethod = paymentMethod;

        return MapToDto(expense);
    }

    public async Task<ExpenseDto> UpdateAsync(Guid id, Guid userId, UpdateExpenseDto dto)
    {
        if (dto.Amount <= 0)
        {
            throw new ArgumentException("El monto debe ser mayor a cero.");
        }

        var expense = await _expenseRepository.GetByIdAsync(id, userId);
        if (expense == null)
        {
            throw new KeyNotFoundException("Gasto no encontrado.");
        }

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, userId);
        if (category == null)
        {
            throw new KeyNotFoundException("Categoría no encontrada.");
        }

        if (!category.IsActive)
        {
            throw new InvalidOperationException("La categoría no está activa.");
        }

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(dto.PaymentMethodId, userId);
        if (paymentMethod == null)
        {
            throw new KeyNotFoundException("Método de pago no encontrado.");
        }

        expense.CategoryId = dto.CategoryId;
        expense.PaymentMethodId = dto.PaymentMethodId;
        expense.Amount = dto.Amount;
        expense.Date = dto.Date;
        expense.Description = dto.Description;
        expense.UpdatedAt = DateTime.UtcNow;

        expense = await _expenseRepository.UpdateAsync(expense);

        // Load navigation properties
        expense.Category = category;
        expense.PaymentMethod = paymentMethod;

        return MapToDto(expense);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, userId);
        if (expense == null)
        {
            throw new KeyNotFoundException("Gasto no encontrado.");
        }

        await _expenseRepository.DeleteAsync(id, userId);
    }

    private static ExpenseDto MapToDto(Expense expense)
    {
        return new ExpenseDto
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Date = expense.Date,
            Description = expense.Description,
            Category = new CategoryInfoDto
            {
                Id = expense.Category.Id,
                Name = expense.Category.Name
            },
            PaymentMethod = new PaymentMethodInfoDto
            {
                Id = expense.PaymentMethod.Id,
                Name = expense.PaymentMethod.Name
            },
            CreatedAt = expense.CreatedAt
        };
    }
}

