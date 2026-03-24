using GastosPersonales.Application.DTOs.PaymentMethod;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;

namespace GastosPersonales.Application.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetAllByUserIdAsync(Guid userId)
    {
        var paymentMethods = await _paymentMethodRepository.GetAllByUserIdAsync(userId);
        return paymentMethods.Select(pm => new PaymentMethodDto
        {
            Id = pm.Id,
            Name = pm.Name,
            Icon = pm.Icon,
            CreatedAt = pm.CreatedAt
        });
    }

    public async Task<PaymentMethodDto> GetByIdAsync(Guid id, Guid userId)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id, userId);
        if (paymentMethod == null)
        {
            throw new KeyNotFoundException("Método de pago no encontrado.");
        }

        return new PaymentMethodDto
        {
            Id = paymentMethod.Id,
            Name = paymentMethod.Name,
            Icon = paymentMethod.Icon,
            CreatedAt = paymentMethod.CreatedAt
        };
    }

    public async Task<PaymentMethodDto> CreateAsync(Guid userId, CreatePaymentMethodDto dto)
    {
        if (await _paymentMethodRepository.ExistsByNameAsync(dto.Name, userId))
        {
            throw new InvalidOperationException("Ya existe un método de pago con ese nombre.");
        }

        var paymentMethod = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            Icon = dto.Icon,
            CreatedAt = DateTime.UtcNow
        };

        paymentMethod = await _paymentMethodRepository.CreateAsync(paymentMethod);

        return new PaymentMethodDto
        {
            Id = paymentMethod.Id,
            Name = paymentMethod.Name,
            Icon = paymentMethod.Icon,
            CreatedAt = paymentMethod.CreatedAt
        };
    }

    public async Task<PaymentMethodDto> UpdateAsync(Guid id, Guid userId, UpdatePaymentMethodDto dto)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id, userId);
        if (paymentMethod == null)
        {
            throw new KeyNotFoundException("Método de pago no encontrado.");
        }

        if (await _paymentMethodRepository.ExistsByNameAsync(dto.Name, userId, id))
        {
            throw new InvalidOperationException("Ya existe un método de pago con ese nombre.");
        }

        paymentMethod.Name = dto.Name;
        paymentMethod.Icon = dto.Icon;
        paymentMethod.UpdatedAt = DateTime.UtcNow;

        paymentMethod = await _paymentMethodRepository.UpdateAsync(paymentMethod);

        return new PaymentMethodDto
        {
            Id = paymentMethod.Id,
            Name = paymentMethod.Name,
            Icon = paymentMethod.Icon,
            CreatedAt = paymentMethod.CreatedAt
        };
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id, userId);
        if (paymentMethod == null)
        {
            throw new KeyNotFoundException("Método de pago no encontrado.");
        }

        await _paymentMethodRepository.DeleteAsync(id, userId);
    }
}

