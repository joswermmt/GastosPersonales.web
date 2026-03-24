namespace GastosPersonales.Application.DTOs.Budget;

public class CreateBudgetDto
{
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

