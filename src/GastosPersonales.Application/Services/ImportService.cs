using System.Text;
using System.Text.Json;
using GastosPersonales.Application.DTOs.Expense;
using GastosPersonales.Application.DTOs.Import;
using GastosPersonales.Domain.Repositories;

namespace GastosPersonales.Application.Services;

public class ImportService : IImportService
{
    private readonly IExpenseService _expenseService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public ImportService(
        IExpenseService expenseService,
        ICategoryRepository categoryRepository,
        IPaymentMethodRepository paymentMethodRepository)
    {
        _expenseService = expenseService;
        _categoryRepository = categoryRepository;
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<ImportResultDto> ImportFromCsvAsync(Stream fileStream, Guid userId)
    {
        var result = new ImportResultDto();
        var errors = new List<ImportErrorDto>();

        using var reader = new StreamReader(fileStream, Encoding.UTF8);
        var lineNumber = 0;

        // Skip header
        await reader.ReadLineAsync();
        lineNumber++;

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;
            result.TotalRows++;

            try
            {
                var parts = line.Split(',');
                if (parts.Length < 5)
                {
                    errors.Add(new ImportErrorDto
                    {
                        RowNumber = lineNumber,
                        Message = "Formato inválido: se requieren al menos 5 columnas",
                        Data = new Dictionary<string, string> { { "line", line } }
                    });
                    result.ErrorCount++;
                    continue;
                }

                var expenseDto = new CreateExpenseDto
                {
                    Amount = decimal.Parse(parts[0].Trim()),
                    Date = DateTime.Parse(parts[1].Trim()),
                    CategoryId = Guid.Parse(parts[2].Trim()),
                    PaymentMethodId = Guid.Parse(parts[3].Trim()),
                    Description = parts.Length > 4 ? parts[4].Trim() : null
                };

                // Validate category and payment method belong to user
                var category = await _categoryRepository.GetByIdAsync(expenseDto.CategoryId, userId);
                if (category == null || !category.IsActive)
                {
                    errors.Add(new ImportErrorDto
                    {
                        RowNumber = lineNumber,
                        Message = "Categoría no encontrada o inactiva",
                        Data = new Dictionary<string, string> { { "categoryId", expenseDto.CategoryId.ToString() } }
                    });
                    result.ErrorCount++;
                    continue;
                }

                var paymentMethod = await _paymentMethodRepository.GetByIdAsync(expenseDto.PaymentMethodId, userId);
                if (paymentMethod == null)
                {
                    errors.Add(new ImportErrorDto
                    {
                        RowNumber = lineNumber,
                        Message = "Método de pago no encontrado",
                        Data = new Dictionary<string, string> { { "paymentMethodId", expenseDto.PaymentMethodId.ToString() } }
                    });
                    result.ErrorCount++;
                    continue;
                }

                await _expenseService.CreateAsync(userId, expenseDto);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                errors.Add(new ImportErrorDto
                {
                    RowNumber = lineNumber,
                    Message = ex.Message,
                    Data = new Dictionary<string, string> { { "line", line } }
                });
                result.ErrorCount++;
            }
        }

        result.Errors = errors;
        return result;
    }

    public async Task<ImportResultDto> ImportFromExcelAsync(Stream fileStream, Guid userId)
    {
        // For Excel, we'll use a simple approach with EPPlus or ClosedXML
        // For now, treating as CSV-like format
        // In production, use EPPlus or ClosedXML library
        return await ImportFromCsvAsync(fileStream, userId);
    }

    public async Task<ImportResultDto> ImportFromJsonAsync(Stream fileStream, Guid userId)
    {
        var result = new ImportResultDto();
        var errors = new List<ImportErrorDto>();

        try
        {
            using var reader = new StreamReader(fileStream, Encoding.UTF8);
            var jsonContent = await reader.ReadToEndAsync();
            var expenses = JsonSerializer.Deserialize<List<CreateExpenseDto>>(jsonContent);

            if (expenses == null)
            {
                result.Errors.Add(new ImportErrorDto
                {
                    RowNumber = 0,
                    Message = "El archivo JSON está vacío o tiene formato inválido"
                });
                result.ErrorCount = 1;
                return result;
            }

            result.TotalRows = expenses.Count;

            for (int i = 0; i < expenses.Count; i++)
            {
                var expenseDto = expenses[i];
                var rowNumber = i + 1;

                try
                {
                    // Validate category and payment method belong to user
                    var category = await _categoryRepository.GetByIdAsync(expenseDto.CategoryId, userId);
                    if (category == null || !category.IsActive)
                    {
                        errors.Add(new ImportErrorDto
                        {
                            RowNumber = rowNumber,
                            Message = "Categoría no encontrada o inactiva",
                            Data = new Dictionary<string, string> { { "categoryId", expenseDto.CategoryId.ToString() } }
                        });
                        result.ErrorCount++;
                        continue;
                    }

                    var paymentMethod = await _paymentMethodRepository.GetByIdAsync(expenseDto.PaymentMethodId, userId);
                    if (paymentMethod == null)
                    {
                        errors.Add(new ImportErrorDto
                        {
                            RowNumber = rowNumber,
                            Message = "Método de pago no encontrado",
                            Data = new Dictionary<string, string> { { "paymentMethodId", expenseDto.PaymentMethodId.ToString() } }
                        });
                        result.ErrorCount++;
                        continue;
                    }

                    await _expenseService.CreateAsync(userId, expenseDto);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new ImportErrorDto
                    {
                        RowNumber = rowNumber,
                        Message = ex.Message,
                        Data = new Dictionary<string, string>
                        {
                            { "amount", expenseDto.Amount.ToString() },
                            { "date", expenseDto.Date.ToString() }
                        }
                    });
                    result.ErrorCount++;
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = 0,
                Message = $"Error al procesar el archivo JSON: {ex.Message}"
            });
            result.ErrorCount++;
        }

        result.Errors = errors;
        return result;
    }
}

