using System.Security.Claims;
using GastosPersonales.Application.DTOs.Budget;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosPersonales.API.Controllers;

[ApiController]
[Route("api/budgets")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
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
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetAll([FromQuery] int? month = null, [FromQuery] int? year = null)
    {
        try
        {
            var userId = GetUserId();
            IEnumerable<BudgetDto> budgets;

            if (month.HasValue && year.HasValue)
            {
                budgets = await _budgetService.GetByMonthAsync(month.Value, year.Value, userId);
            }
            else
            {
                budgets = await _budgetService.GetAllByUserIdAsync(userId);
            }

            return Ok(budgets);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener presupuestos", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetDto>> GetById(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var budget = await _budgetService.GetByIdAsync(id, userId);
            return Ok(budget);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener presupuesto", error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<BudgetDto>> Create([FromBody] CreateBudgetDto dto)
    {
        try
        {
            var userId = GetUserId();
            var budget = await _budgetService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = budget.Id }, budget);
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
            return StatusCode(500, new { message = "Error al crear presupuesto", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetDto>> Update(Guid id, [FromBody] UpdateBudgetDto dto)
    {
        try
        {
            var userId = GetUserId();
            var budget = await _budgetService.UpdateAsync(id, userId, dto);
            return Ok(budget);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar presupuesto", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetUserId();
            await _budgetService.DeleteAsync(id, userId);
            return Ok(new { message = "Presupuesto eliminado correctamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al eliminar presupuesto", error = ex.Message });
        }
    }
}

