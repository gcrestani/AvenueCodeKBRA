namespace AvenueCodeKBRA.Models;

public class ConversionRequest
{
    public JsonInputModel Input { get; set; } = new();
}

public class ConversionResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? XmlContent { get; set; }
}
