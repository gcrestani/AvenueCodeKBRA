using System.Text.Json.Serialization;

namespace AvenueCodeKBRA.Models;

public class JsonInputModel
{
    [JsonPropertyName("Id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("ReportMetadata")]
    public ReportMetadata ReportMetadata { get; set; } = new();

    [JsonPropertyName("CountryIds")]
    public List<string> CountryIds { get; set; } = new();

    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("PublishDate")]
    public DateTime PublishDate { get; set; }

    [JsonPropertyName("Status")]
    public int Status { get; set; }

    [JsonPropertyName("TestRun")]
    public bool TestRun { get; set; }
}

public class ReportMetadata
{
    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("ContactSection")]
    public List<ContactSection> ContactSection { get; set; } = new();
}

public class ContactSection
{
    [JsonPropertyName("ContactInformation")]
    public List<ContactInformation> ContactInformation { get; set; } = new();
}

public class ContactInformation
{
    [JsonPropertyName("ContactHeader")]
    public string ContactHeader { get; set; } = string.Empty;

    [JsonPropertyName("Contacts")]
    public List<Contact> Contacts { get; set; } = new();
}

public class Contact
{
    [JsonPropertyName("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("LastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("Email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("PhoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("Accreditation")]
    public string Accreditation { get; set; } = string.Empty;
}
