namespace GastosPersonales.Application.DTOs.Import;

public class ImportResultDto
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<ImportErrorDto> Errors { get; set; } = new();
}

public class ImportErrorDto
{
    public int RowNumber { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string>? Data { get; set; }
}

