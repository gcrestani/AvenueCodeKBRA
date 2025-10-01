using AvenueCodeKBRA.Models;

namespace AvenueCodeKBRA.Services;

/// <summary>
/// Service interface for XML conversion operations
/// </summary>
public interface IXmlConversionService
{
    /// <summary>
    /// Converts JSON input to XML format with validation
    /// </summary>
    /// <param name="input">The JSON input model</param>
    /// <returns>Conversion result with XML content or error message</returns>
    ConversionResponse ConvertToXml(JsonInputModel input);
}
