using Microsoft.EntityFrameworkCore;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using GastosPersonales.Infrastructure.Data;

namespace GastosPersonales.Infrastructure.Repositories;

public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentMethodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentMethod?> GetByIdAsync(Guid id, Guid userId)
    {
        return await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Id == id && pm.UserId == userId);
    }

    public async Task<IEnumerable<PaymentMethod>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.PaymentMethods
            .Where(pm => pm.UserId == userId)
            .OrderBy(pm => pm.Name)
            .ToListAsync();
    }

    public async Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod)
    {
        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();
        return paymentMethod;
    }

    public async Task<PaymentMethod> UpdateAsync(PaymentMethod paymentMethod)
    {
        _context.PaymentMethods.Update(paymentMethod);
        await _context.SaveChangesAsync();
        return paymentMethod;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Id == id && pm.UserId == userId);
        
        if (paymentMethod == null)
            return false;

        _context.PaymentMethods.Remove(paymentMethod);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid userId, Guid? excludeId = null)
    {
        var query = _context.PaymentMethods
            .Where(pm => pm.UserId == userId && 
                   pm.Name.ToLower() == name.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(pm => pm.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
