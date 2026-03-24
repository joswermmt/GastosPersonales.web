using GastosPersonales.Domain.Entities;

namespace GastosPersonales.Domain.Repositories;

public interface IPaymentMethodRepository
{
    Task<PaymentMethod?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<PaymentMethod>> GetAllByUserIdAsync(Guid userId);
    Task<PaymentMethod> CreateAsync(PaymentMethod paymentMethod);
    Task<PaymentMethod> UpdateAsync(PaymentMethod paymentMethod);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<bool> ExistsByNameAsync(string name, Guid userId, Guid? excludeId = null);
}

