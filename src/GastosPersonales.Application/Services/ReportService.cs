using GastosPersonales.Application.DTOs.Report;
using GastosPersonales.Domain.Repositories;

namespace GastosPersonales.Application.Services;

public class ReportService : IReportService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ReportService(
        IExpenseRepository expenseRepository,
        ICategoryRepository categoryRepository)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<MonthlyReportDto> GetMonthlyReportAsync(int month, int year, Guid userId)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var expenses = await _expenseRepository.GetFilteredAsync(
            userId, startDate, endDate, null, null, null);

        var totalSpent = expenses.Sum(e => e.Amount);

        // Previous month
        var prevStartDate = startDate.AddMonths(-1);
        var prevEndDate = startDate.AddDays(-1);
        var prevExpenses = await _expenseRepository.GetFilteredAsync(
            userId, prevStartDate, prevEndDate, null, null, null);
        var previousMonthTotal = prevExpenses.Sum(e => e.Amount);

        var difference = totalSpent - previousMonthTotal;
        var percentageChange = previousMonthTotal > 0
            ? (double)(difference / previousMonthTotal * 100)
            : 0;

        // Category summaries
        var categorySummaries = expenses
            .GroupBy(e => new { e.Category.Id, e.Category.Name })
            .Select(g => new CategorySummaryDto
            {
                CategoryId = g.Key.Id,
                CategoryName = g.Key.Name,
                Amount = g.Sum(e => e.Amount),
                Percentage = totalSpent > 0 ? (double)(g.Sum(e => e.Amount) / totalSpent * 100) : 0
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        // Top categories
        var topCategories = categorySummaries
            .Take(5)
            .Select(c => new TopCategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Amount = c.Amount
            })
            .ToList();

        return new MonthlyReportDto
        {
            Month = month,
            Year = year,
            TotalSpent = totalSpent,
            PreviousMonthTotal = previousMonthTotal,
            Difference = difference,
            PercentageChange = percentageChange,
            CategorySummaries = categorySummaries,
            TopCategories = topCategories
        };
    }
}

