namespace GastosPersonales.Application.DTOs.Expense;

public class UpdateExpenseDto
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string? Description { get; set; }
}

