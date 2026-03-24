namespace GastosPersonales.Application.DTOs.Expense;

public class ExpenseDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public CategoryInfoDto Category { get; set; } = null!;
    public PaymentMethodInfoDto PaymentMethod { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class CategoryInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PaymentMethodInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

