using Microsoft.AspNetCore.Mvc;
using AvenueCodeKBRA.Models;
using AvenueCodeKBRA.Services;

namespace AvenueCodeKBRA.Controllers;

[ApiController]
[Route("api/[controller]")]
public class XmlConversionController : ControllerBase
{
    private readonly IXmlConversionService _conversionService;
    private readonly ILogger<XmlConversionController> _logger;

    public XmlConversionController(IXmlConversionService conversionService, ILogger<XmlConversionController> logger)
    {
        _conversionService = conversionService;
        _logger = logger;
    }

    [HttpPost("convert")]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status400BadRequest)]
    public IActionResult ConvertToXml([FromBody] JsonInputModel request)
    {
        try
        {
            _logger.LogInformation("Starting XML conversion for document ID: {DocumentId}", request.Id);

            var result = _conversionService.ConvertToXml(request);

            if (result.Success)
            {
                _logger.LogInformation("XML conversion completed successfully for document ID: {DocumentId}", request.Id);
                return Ok(result);
            }

            _logger.LogWarning("XML conversion failed for document ID: {DocumentId}. Error: {Error}", 
                request.Id, result.ErrorMessage);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during XML conversion for document ID: {DocumentId}", request.Id);
            return StatusCode(500, new ConversionResponse
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred during conversion."
            });
        }
    }

    [HttpPost("convert-file")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertToXmlFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ConversionResponse
                {
                    Success = false,
                    ErrorMessage = "No file provided or file is empty."
                });
            }

            if (!file.ContentType.Contains("json") && !file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ConversionResponse
                {
                    Success = false,
                    ErrorMessage = "File must be a JSON file."
                });
            }

            _logger.LogInformation("Starting XML file conversion for file: {FileName}", file.FileName);

            // Read the JSON content from the file
            using var reader = new StreamReader(file.OpenReadStream());
            var jsonContent = await reader.ReadToEndAsync();

            // Deserialize JSON to model
            var jsonModel = System.Text.Json.JsonSerializer.Deserialize<JsonInputModel>(jsonContent, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (jsonModel == null)
            {
                return BadRequest(new ConversionResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid JSON format in the uploaded file."
                });
            }

            // Convert to XML
            var result = _conversionService.ConvertToXml(jsonModel);

            if (!result.Success)
            {
                _logger.LogWarning("XML conversion failed for file: {FileName}. Error: {Error}", 
                    file.FileName, result.ErrorMessage);
                return BadRequest(result);
            }

            // Return XML as downloadable file
            var xmlBytes = System.Text.Encoding.UTF8.GetBytes(result.XmlContent!);
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_converted.xml";

            _logger.LogInformation("XML file conversion completed successfully for file: {FileName}", file.FileName);

            return File(xmlBytes, "application/xml", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during XML file conversion for file: {FileName}", file?.FileName);
            return StatusCode(500, new ConversionResponse
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred during file conversion."
            });
        }
    }
}
