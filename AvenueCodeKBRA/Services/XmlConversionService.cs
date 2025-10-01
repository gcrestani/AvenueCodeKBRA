using AvenueCodeKBRA.Models;
using Microsoft.Extensions.Options;
using System.Xml.Serialization;

namespace AvenueCodeKBRA.Services;

/// <summary>
/// Service implementation for XML conversion operations
/// </summary>
public class XmlConversionService : IXmlConversionService
{
    private readonly ILogger<XmlConversionService> _logger;
    private readonly ApplicationSettings _applicationSettings;

    public XmlConversionService(ILogger<XmlConversionService> logger, IOptions<ApplicationSettings> applicationSettings)
    {
        _logger = logger;
        _applicationSettings = applicationSettings.Value;
    }

    public ConversionResponse ConvertToXml(JsonInputModel input)
    {
        try
        {
            // Validate input
            var validationResult = ValidateInput(input);
            if (!validationResult.IsValid)
            {
                return new ConversionResponse
                {
                    Success = false,
                    ErrorMessage = validationResult.ErrorMessage
                };
            }

            // Convert to XML model
            var xmlModel = ConvertToXmlModel(input);

            // Serialize to XML
            var xmlContent = SerializeToXml(xmlModel);

            return new ConversionResponse
            {
                Success = true,
                XmlContent = xmlContent
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during XML conversion");
            return new ConversionResponse
            {
                Success = false,
                ErrorMessage = "An error occurred during XML conversion."
            };
        }
    }

    private ValidationResult ValidateInput(JsonInputModel input)
    {
        // Check if Status is not equal to 3
        if (input.Status != 3)
        {
            return ValidationResult.Failure("Status must be equal to 3");
        }

        // Check if Publish date is before the configured cutoff date
        if (input.PublishDate < _applicationSettings.CutoffDate)
        {
            return ValidationResult.Failure($"Publish date must be on or after {_applicationSettings.CutoffDate:yyyy-MM-dd}");
        }

        // Check if it's not a test run
        if (!input.TestRun)
        {
            return ValidationResult.Failure("TestRun must be true for production processing");
        }

        return ValidationResult.Success();
    }

    private XmlOutputModel ConvertToXmlModel(JsonInputModel input)
    {
        var xmlModel = new XmlOutputModel
        {
            Title = input.Title,
            Countries = string.Join(",", input.CountryIds),
            PublishedDate = input.PublishDate,
            ContactInformation = new ContactInformationXml
            {
                PersonGroup = new PersonGroup
                {
                    Sequence = _applicationSettings.XmlOutput.PersonGroupSequence,
                    Name = _applicationSettings.XmlOutput.PersonGroupName,
                    PersonGroupMember = new PersonGroupMember
                    {
                        Person = CreatePersonsFromContacts(input.ReportMetadata.ContactSection)
                    }
                }
            }
        };

        return xmlModel;
    }

    private List<Person> CreatePersonsFromContacts(List<ContactSection> contactSections)
    {
        var persons = new List<Person>();

        // Group contacts by name to handle multiple contact methods for the same person
        var contactGroups = contactSections
            .SelectMany(cs => cs.ContactInformation)
            .SelectMany(ci => ci.Contacts)
            .Where(c => !string.IsNullOrEmpty(c.FirstName) && !string.IsNullOrEmpty(c.LastName))
            .GroupBy(c => $"{c.FirstName.Trim()} {c.LastName.Trim()}", StringComparer.OrdinalIgnoreCase);

        foreach (var group in contactGroups)
        {
            var contacts = group.ToList();
            var primaryContact = contacts.First();

            var person = new Person
            {
                FamilyName = primaryContact.LastName,
                GivenName = primaryContact.FirstName,
                DisplayName = $"{primaryContact.FirstName} {primaryContact.LastName}",
                JobTitle = primaryContact.Title,
                ContactInfo = new ContactInfo
                {
                    Phone = contacts
                        .Where(c => !string.IsNullOrEmpty(c.PhoneNumber))
                        .Select(c => new Phone { Number = c.PhoneNumber })
                        .ToList(),
                    Email = contacts
                        .Where(c => !string.IsNullOrEmpty(c.Email))
                        .Select(c => new Email { Address = c.Email })
                        .ToList()
                }
            };

            persons.Add(person);
        }

        return persons;
    }

    private string SerializeToXml(XmlOutputModel model)
    {
        var serializer = new XmlSerializer(typeof(XmlOutputModel));
        using var stringWriter = new StringWriter();
        
        // Create XmlWriterSettings to control XML formatting
        var settings = new System.Xml.XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            NewLineChars = "\r\n",
            NewLineHandling = System.Xml.NewLineHandling.Replace,
            OmitXmlDeclaration = false, // Let the serializer handle the declaration
            Encoding = System.Text.Encoding.UTF8
        };
        
        using var xmlWriter = System.Xml.XmlWriter.Create(stringWriter, settings);
        serializer.Serialize(xmlWriter, model);
        
        return stringWriter.ToString();
    }
}
