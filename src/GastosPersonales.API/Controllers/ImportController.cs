using System.Security.Claims;
using GastosPersonales.Application.DTOs.Import;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GastosPersonales.API.Controllers;

[ApiController]
[Route("api/import")]
[Authorize]
public class ImportController : ControllerBase
{
    private readonly IImportService _importService;

    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Usuario no autenticado.");
        }
        return userId;
    }

    [HttpPost("csv")]
    public async Task<ActionResult<ImportResultDto>> ImportCsv(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Archivo no válido." });
            }

            var userId = GetUserId();
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportFromCsvAsync(stream, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al importar archivo", error = ex.Message });
        }
    }

    [HttpPost("excel")]
    public async Task<ActionResult<ImportResultDto>> ImportExcel(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Archivo no válido." });
            }

            var userId = GetUserId();
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportFromExcelAsync(stream, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al importar archivo", error = ex.Message });
        }
    }

    [HttpPost("json")]
    public async Task<ActionResult<ImportResultDto>> ImportJson(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Archivo no válido." });
            }

            var userId = GetUserId();
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportFromJsonAsync(stream, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al importar archivo", error = ex.Message });
        }
    }
}

