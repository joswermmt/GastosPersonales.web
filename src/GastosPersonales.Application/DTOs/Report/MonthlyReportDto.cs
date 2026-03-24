namespace GastosPersonales.Application.DTOs.Report;

public class MonthlyReportDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal PreviousMonthTotal { get; set; }
    public decimal Difference { get; set; }
    public double PercentageChange { get; set; }
    public List<CategorySummaryDto> CategorySummaries { get; set; } = new();
    public List<TopCategoryDto> TopCategories { get; set; } = new();
}

public class CategorySummaryDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public double Percentage { get; set; }
}

public class TopCategoryDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

