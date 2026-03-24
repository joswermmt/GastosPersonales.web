using Microsoft.EntityFrameworkCore;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using GastosPersonales.Infrastructure.Data;

namespace GastosPersonales.Infrastructure.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly ApplicationDbContext _context;

    public BudgetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Budget?> GetByIdAsync(Guid id, Guid userId)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
    }

    public async Task<Budget?> GetByCategoryAndMonthAsync(Guid categoryId, int month, int year, Guid userId)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.CategoryId == categoryId &&
                                     b.Month == month &&
                                     b.Year == year &&
                                     b.UserId == userId);
    }

    public async Task<IEnumerable<Budget>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.User)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetByMonthAsync(int month, int year, Guid userId)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Include(b => b.User)
            .Where(b => b.Month == month && b.Year == year && b.UserId == userId)
            .ToListAsync();
    }

    public async Task<Budget> CreateAsync(Budget budget)
    {
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();
        
        await _context.Entry(budget)
            .Reference(b => b.Category)
            .LoadAsync();
        await _context.Entry(budget)
            .Reference(b => b.User)
            .LoadAsync();
        
        return budget;
    }

    public async Task<Budget> UpdateAsync(Budget budget)
    {
        _context.Budgets.Update(budget);
        await _context.SaveChangesAsync();
        
        await _context.Entry(budget)
            .Reference(b => b.Category)
            .LoadAsync();
        await _context.Entry(budget)
            .Reference(b => b.User)
            .LoadAsync();
        
        return budget;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
        
        if (budget == null)
            return false;

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();
        return true;
    }
}
