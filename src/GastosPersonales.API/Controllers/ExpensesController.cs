using System.Security.Claims;
using GastosPersonales.Application.DTOs.Expense;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosPersonales.API.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
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
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetAll([FromQuery] ExpenseFilterDto? filter = null)
    {
        try
        {
            var userId = GetUserId();
            IEnumerable<ExpenseDto> expenses;

            if (filter != null && (filter.StartDate.HasValue || filter.EndDate.HasValue ||
                filter.CategoryId.HasValue || filter.PaymentMethodId.HasValue ||
                !string.IsNullOrWhiteSpace(filter.SearchText)))
            {
                expenses = await _expenseService.GetFilteredAsync(userId, filter);
            }
            else
            {
                expenses = await _expenseService.GetAllByUserIdAsync(userId);
            }

            return Ok(expenses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener gastos", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetById(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var expense = await _expenseService.GetByIdAsync(id, userId);
            return Ok(expense);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener gasto", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> Create([FromBody] CreateExpenseDto dto)
    {
        try
        {
            var userId = GetUserId();
            var expense = await _expenseService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
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
            return StatusCode(500, new { message = "Error al crear gasto", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ExpenseDto>> Update(Guid id, [FromBody] UpdateExpenseDto dto)
    {
        try
        {
            var userId = GetUserId();
            var expense = await _expenseService.UpdateAsync(id, userId, dto);
            return Ok(expense);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar gasto", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetUserId();
            await _expenseService.DeleteAsync(id, userId);
            return Ok(new { message = "Gasto eliminado correctamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar gasto", error = ex.Message });
        }
    }
}

