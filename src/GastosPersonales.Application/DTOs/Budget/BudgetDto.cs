namespace GastosPersonales.Application.DTOs.Budget;

public class BudgetDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public double PercentageUsed { get; set; }
    public string AlertLevel { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
}

