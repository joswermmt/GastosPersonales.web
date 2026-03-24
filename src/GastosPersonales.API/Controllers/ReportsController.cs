using System.Security.Claims;
using GastosPersonales.Application.DTOs.Report;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosPersonales.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
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

    [HttpGet("monthly")]
    public async Task<ActionResult<MonthlyReportDto>> GetMonthlyReport([FromQuery] int month, [FromQuery] int year)
    {
        try
        {
            var userId = GetUserId();
            var report = await _reportService.GetMonthlyReportAsync(month, year, userId);
            return Ok(report);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al generar reporte", error = ex.Message });
        }
    }
}

