using System.Security.Claims;
using GastosPersonales.Application.DTOs.PaymentMethod;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosPersonales.API.Controllers;

[ApiController]
[Route("api/payment-methods")]
[Authorize]
public class PaymentMethodsController : ControllerBase
{
    private readonly IPaymentMethodService _paymentMethodService;

    public PaymentMethodsController(IPaymentMethodService paymentMethodService)
    {
        _paymentMethodService = paymentMethodService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Usuario no autenticado.");
        }
        return userId;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetAll()
    {
        try
        {
            var userId = GetUserId();
            var paymentMethods = await _paymentMethodService.GetAllByUserIdAsync(userId);
            return Ok(paymentMethods);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener métodos de pago", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentMethodDto>> GetById(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var paymentMethod = await _paymentMethodService.GetByIdAsync(id, userId);
            return Ok(paymentMethod);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener método de pago", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<PaymentMethodDto>> Create([FromBody] CreatePaymentMethodDto dto)
    {
        try
        {
            var userId = GetUserId();
            var paymentMethod = await _paymentMethodService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = paymentMethod.Id }, paymentMethod);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al crear método de pago", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PaymentMethodDto>> Update(Guid id, [FromBody] UpdatePaymentMethodDto dto)
    {
        try
        {
            var userId = GetUserId();
            var paymentMethod = await _paymentMethodService.UpdateAsync(id, userId, dto);
            return Ok(paymentMethod);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar método de pago", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetUserId();
            await _paymentMethodService.DeleteAsync(id, userId);
            return Ok(new { message = "Método de pago eliminado correctamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar método de pago", error = ex.Message });
        }
    }
}

