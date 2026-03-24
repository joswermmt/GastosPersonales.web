using Microsoft.EntityFrameworkCore;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using GastosPersonales.Infrastructure.Data;

namespace GastosPersonales.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _context;

    public ExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Expense?> GetByIdAsync(Guid id, Guid userId)
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .Include(e => e.PaymentMethod)
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
    }

    public async Task<IEnumerable<Expense>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .Include(e => e.PaymentMethod)
            .Include(e => e.User)
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetFilteredAsync(
        Guid userId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? categoryId = null,
        Guid? paymentMethodId = null,
        string? searchText = null)
    {
        var query = _context.Expenses
            .Include(e => e.Category)
            .Include(e => e.PaymentMethod)
            .Include(e => e.User)
            .Where(e => e.UserId == userId);

        if (startDate.HasValue)
        {
            query = query.Where(e => e.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(e => e.Date <= endDate.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == categoryId.Value);
        }

        if (paymentMethodId.HasValue)
        {
            query = query.Where(e => e.PaymentMethodId == paymentMethodId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(e => e.Description != null && 
                e.Description.Contains(searchText));
        }

        return await query
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        
        await _context.Entry(expense)
            .Reference(e => e.Category)
            .LoadAsync();
        await _context.Entry(expense)
            .Reference(e => e.PaymentMethod)
            .LoadAsync();
        await _context.Entry(expense)
            .Reference(e => e.User)
            .LoadAsync();
        
        return expense;
    }

    public async Task<Expense> UpdateAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
        
        await _context.Entry(expense)
            .Reference(e => e.Category)
            .LoadAsync();
        await _context.Entry(expense)
            .Reference(e => e.PaymentMethod)
            .LoadAsync();
        await _context.Entry(expense)
            .Reference(e => e.User)
            .LoadAsync();
        
        return expense;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        
        if (expense == null)
            return false;

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> GetTotalByCategoryAndMonthAsync(Guid categoryId, int month, int year)
    {
        return await _context.Expenses
            .Where(e => e.CategoryId == categoryId && 
                   e.Date.Month == month && 
                   e.Date.Year == year)
            .SumAsync(e => e.Amount);
    }

    public async Task<IEnumerable<Expense>> GetByCategoryAndMonthAsync(Guid categoryId, int month, int year)
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .Include(e => e.PaymentMethod)
            .Where(e => e.CategoryId == categoryId && 
                   e.Date.Month == month && 
                   e.Date.Year == year)
            .ToListAsync();
    }
}
