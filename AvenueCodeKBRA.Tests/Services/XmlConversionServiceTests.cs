using AvenueCodeKBRA.Models;
using AvenueCodeKBRA.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using FluentAssertions;
using Xunit;

namespace AvenueCodeKBRA.Tests.Services;

public class XmlConversionServiceTests
{
    private readonly Mock<ILogger<XmlConversionService>> _mockLogger;
    private readonly Mock<IOptions<ApplicationSettings>> _mockOptions;
    private readonly ApplicationSettings _applicationSettings;
    private readonly XmlConversionService _service;

    public XmlConversionServiceTests()
    {
        _mockLogger = new Mock<ILogger<XmlConversionService>>();
        _mockOptions = new Mock<IOptions<ApplicationSettings>>();
        _applicationSettings = new ApplicationSettings
        {
            CutoffDate = new DateTime(2024, 1, 1)
        };
        _mockOptions.Setup(x => x.Value).Returns(_applicationSettings);
        _service = new XmlConversionService(_mockLogger.Object, _mockOptions.Object);
    }

    [Fact]
    public void ConvertToXml_ValidInput_ReturnsSuccessResponse()
    {
        // Arrange
        var input = CreateValidJsonInputModel();

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.XmlContent.Should().NotBeNullOrEmpty();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void ConvertToXml_StatusNotEqualTo3_ReturnsFailureResponse()
    {
        // Arrange
        var input = CreateValidJsonInputModel();
        input.Status = 2; // Invalid status

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Status must be equal to 3");
        result.XmlContent.Should().BeNull();
    }

    [Fact]
    public void ConvertToXml_PublishDateBeforeCutoff_ReturnsFailureResponse()
    {
        // Arrange
        var input = CreateValidJsonInputModel();
        input.PublishDate = new DateTime(2023, 12, 31); // Before cutoff date

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Publish date must be on or after 2024-01-01");
        result.XmlContent.Should().BeNull();
    }

    [Fact]
    public void ConvertToXml_TestRunIsFalse_ReturnsFailureResponse()
    {
        // Arrange
        var input = CreateValidJsonInputModel();
        input.TestRun = false;

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("TestRun must be true for production processing");
        result.XmlContent.Should().BeNull();
    }

    [Fact]
    public void ConvertToXml_ValidInput_GeneratesCorrectXmlStructure()
    {
        // Arrange
        var input = CreateValidJsonInputModel();

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Success.Should().BeTrue();
        result.XmlContent.Should().Contain("<PublishedItem");
        result.XmlContent.Should().Contain($"<Title>{input.Title}</Title>");
        result.XmlContent.Should().Contain($"<Countries>{string.Join(",", input.CountryIds)}</Countries>");
        result.XmlContent.Should().Contain("<ContactInformation>");
        result.XmlContent.Should().Contain("<PersonGroup");
    }

    [Fact]
    public void ConvertToXml_WithContactInformation_IncludesPersonDetails()
    {
        // Arrange
        var input = CreateValidJsonInputModel();
        input.ReportMetadata.ContactSection = new List<ContactSection>
        {
            new ContactSection
            {
                ContactInformation = new List<ContactInformation>
                {
                    new ContactInformation
                    {
                        Contacts = new List<Contact>
                        {
                            new Contact
                            {
                                FirstName = "John",
                                LastName = "Doe",
                                Title = "Senior Analyst",
                                PhoneNumber = "+1-555-123-4567"
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Success.Should().BeTrue();
        result.XmlContent.Should().Contain("<FamilyName>Doe</FamilyName>");
        result.XmlContent.Should().Contain("<GivenName>John</GivenName>");
        result.XmlContent.Should().Contain("<DisplayName>John Doe</DisplayName>");
        result.XmlContent.Should().Contain("<JobTitle>Senior Analyst</JobTitle>");
        result.XmlContent.Should().Contain("<Number>+1-555-123-4567</Number>");
    }

    [Fact]
    public void ConvertToXml_WithEmptyContactInformation_HandlesGracefully()
    {
        // Arrange
        var input = CreateValidJsonInputModel();
        input.ReportMetadata.ContactSection = new List<ContactSection>();

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Success.Should().BeTrue();
        result.XmlContent.Should().Contain("<PersonGroup");
        result.XmlContent.Should().Contain("<PersonGroupMember />");
    }

    [Fact]
    public void ConvertToXml_WithInvalidContactInformation_HandlesGracefully()
    {
        // Arrange
        var input = CreateValidJsonInputModel();
        input.ReportMetadata.ContactSection = new List<ContactSection>
        {
            new ContactSection
            {
                ContactInformation = new List<ContactInformation>
                {
                    new ContactInformation
                    {
                        Contacts = new List<Contact>
                        {
                            new Contact
                            {
                                FirstName = "", // Empty first name
                                LastName = "", // Empty last name
                                Title = "Analyst",
                                PhoneNumber = "+1-555-123-4567"
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Success.Should().BeTrue();
        result.XmlContent.Should().Contain("<PersonGroup");
        result.XmlContent.Should().Contain("<PersonGroupMember />");
    }

    [Fact]
    public void ConvertToXml_WithMultipleContactsForSamePerson_ConsolidatesContactMethods()
    {
        // Arrange
        var input = CreateValidJsonInputModel();
        input.ReportMetadata.ContactSection = new List<ContactSection>
        {
            new ContactSection
            {
                ContactInformation = new List<ContactInformation>
                {
                    new ContactInformation
                    {
                        Contacts = new List<Contact>
                        {
                            new Contact
                            {
                                FirstName = "John",
                                LastName = "Doe",
                                Email = "john.doe@work.com",
                                Title = "Senior Analyst",
                                PhoneNumber = "+1-555-123-4567"
                            },
                            new Contact
                            {
                                FirstName = "John",
                                LastName = "Doe",
                                Email = "john.doe@personal.com",
                                Title = "Senior Analyst",
                                PhoneNumber = "+1-555-987-6543"
                            }
                        }
                    }
                }
            }
        };

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Success.Should().BeTrue();
        result.XmlContent.Should().Contain("<FamilyName>Doe</FamilyName>");
        result.XmlContent.Should().Contain("<GivenName>John</GivenName>");
        result.XmlContent.Should().Contain("<DisplayName>John Doe</DisplayName>");
        result.XmlContent.Should().Contain("<JobTitle>Senior Analyst</JobTitle>");
        // Should contain both phone numbers
        result.XmlContent.Should().Contain("<Number>+1-555-123-4567</Number>");
        result.XmlContent.Should().Contain("<Number>+1-555-987-6543</Number>");
        // Should contain both email addresses
        result.XmlContent.Should().Contain("<Address>john.doe@work.com</Address>");
        result.XmlContent.Should().Contain("<Address>john.doe@personal.com</Address>");
    }

    private static JsonInputModel CreateValidJsonInputModel()
    {
        return new JsonInputModel
        {
            Id = "test-doc-001",
            Title = "Test Document",
            CountryIds = new List<string> { "US", "CA", "MX" },
            PublishDate = new DateTime(2024, 6, 15),
            Status = 3,
            TestRun = true,
            ReportMetadata = new ReportMetadata
            {
                Title = "Test Report",
                ContactSection = new List<ContactSection>
                {
                    new ContactSection
                    {
                        ContactInformation = new List<ContactInformation>
                        {
                            new ContactInformation
                            {
                                ContactHeader = "Primary Contacts",
                                Contacts = new List<Contact>
                                {
                                    new Contact
                                    {
                                        FirstName = "Jane",
                                        LastName = "Smith",
                                        Email = "jane.smith@example.com",
                                        Title = "Lead Analyst",
                                        PhoneNumber = "+1-555-987-6543",
                                        Accreditation = "CFA"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
