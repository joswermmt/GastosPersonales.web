using GastosPersonales.Application.DTOs.Import;

namespace GastosPersonales.Application.Services;

public interface IImportService
{
    Task<ImportResultDto> ImportFromCsvAsync(Stream fileStream, Guid userId);
    Task<ImportResultDto> ImportFromExcelAsync(Stream fileStream, Guid userId);
    Task<ImportResultDto> ImportFromJsonAsync(Stream fileStream, Guid userId);
}

