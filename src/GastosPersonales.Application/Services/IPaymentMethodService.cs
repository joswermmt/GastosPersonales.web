using GastosPersonales.Application.DTOs.PaymentMethod;

namespace GastosPersonales.Application.Services;

public interface IPaymentMethodService
{
    Task<IEnumerable<PaymentMethodDto>> GetAllByUserIdAsync(Guid userId);
    Task<PaymentMethodDto> GetByIdAsync(Guid id, Guid userId);
    Task<PaymentMethodDto> CreateAsync(Guid userId, CreatePaymentMethodDto dto);
    Task<PaymentMethodDto> UpdateAsync(Guid id, Guid userId, UpdatePaymentMethodDto dto);
    Task DeleteAsync(Guid id, Guid userId);
}

