namespace GastosPersonales.Application.DTOs.Expense;

public class ExpenseFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? PaymentMethodId { get; set; }
    public string? SearchText { get; set; }
}

