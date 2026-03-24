using System.Security.Claims;
using System.Text;
using System.Text.Json;
using GastosPersonales.Application.DTOs.Report;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosPersonales.API.Controllers;

[ApiController]
[Route("api/export")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ExportController(IReportService reportService)
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

    [HttpGet("excel")]
    public async Task<IActionResult> ExportExcel([FromQuery] int month, [FromQuery] int year)
    {
        try
        {
            var userId = GetUserId();
            var report = await _reportService.GetMonthlyReportAsync(month, year, userId);

            // Simple CSV export (in production, use EPPlus or ClosedXML for real Excel)
            var csv = new StringBuilder();
            csv.AppendLine($"Reporte Mensual - {month}/{year}");
            csv.AppendLine($"Total Gastado,{report.TotalSpent}");
            csv.AppendLine($"Mes Anterior,{report.PreviousMonthTotal}");
            csv.AppendLine($"Diferencia,{report.Difference}");
            csv.AppendLine($"Cambio Porcentual,{report.PercentageChange:F2}%");
            csv.AppendLine();
            csv.AppendLine("Categoría,Monto,Porcentaje");
            foreach (var category in report.CategorySummaries)
            {
                csv.AppendLine($"{category.CategoryName},{category.Amount},{category.Percentage:F2}%");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"reporte_{month}_{year}.csv");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al exportar reporte", error = ex.Message });
        }
    }

    [HttpGet("txt")]
    public async Task<IActionResult> ExportTxt([FromQuery] int month, [FromQuery] int year)
    {
        try
        {
            var userId = GetUserId();
            var report = await _reportService.GetMonthlyReportAsync(month, year, userId);

            var txt = new StringBuilder();
            txt.AppendLine($"REPORTE MENSUAL - {month}/{year}");
            txt.AppendLine(new string('=', 50));
            txt.AppendLine($"Total Gastado: ${report.TotalSpent:F2}");
            txt.AppendLine($"Mes Anterior: ${report.PreviousMonthTotal:F2}");
            txt.AppendLine($"Diferencia: ${report.Difference:F2}");
            txt.AppendLine($"Cambio Porcentual: {report.PercentageChange:F2}%");
            txt.AppendLine();
            txt.AppendLine("DESGLOSE POR CATEGORÍA:");
            txt.AppendLine(new string('-', 50));
            foreach (var category in report.CategorySummaries)
            {
                txt.AppendLine($"{category.CategoryName}: ${category.Amount:F2} ({category.Percentage:F2}%)");
            }
            txt.AppendLine();
            txt.AppendLine("TOP CATEGORÍAS:");
            txt.AppendLine(new string('-', 50));
            foreach (var top in report.TopCategories)
            {
                txt.AppendLine($"{top.CategoryName}: ${top.Amount:F2}");
            }

            var bytes = Encoding.UTF8.GetBytes(txt.ToString());
            return File(bytes, "text/plain", $"reporte_{month}_{year}.txt");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al exportar reporte", error = ex.Message });
        }
    }

    [HttpGet("json")]
    public async Task<IActionResult> ExportJson([FromQuery] int month, [FromQuery] int year)
    {
        try
        {
            var userId = GetUserId();
            var report = await _reportService.GetMonthlyReportAsync(month, year, userId);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(report, options);
            var bytes = Encoding.UTF8.GetBytes(json);

            return File(bytes, "application/json", $"reporte_{month}_{year}.json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al exportar reporte", error = ex.Message });
        }
    }
}

