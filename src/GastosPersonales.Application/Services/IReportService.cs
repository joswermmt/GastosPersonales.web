using GastosPersonales.Application.DTOs.Report;

namespace GastosPersonales.Application.Services;

public interface IReportService
{
    Task<MonthlyReportDto> GetMonthlyReportAsync(int month, int year, Guid userId);
}

