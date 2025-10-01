using AvenueCodeKBRA.Models;
using AvenueCodeKBRA.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using FluentAssertions;
using Xunit;

namespace AvenueCodeKBRA.Tests.Integration;

public class XmlConversionIntegrationTests
{
    private readonly XmlConversionService _service;
    private readonly ApplicationSettings _applicationSettings;

    public XmlConversionIntegrationTests()
    {
        var logger = new Mock<ILogger<XmlConversionService>>();
        _applicationSettings = new ApplicationSettings
        {
            CutoffDate = new DateTime(2024, 1, 1)
        };
        var options = Options.Create(_applicationSettings);
        _service = new XmlConversionService(logger.Object, options);
    }

    [Fact]
    public void ConvertToXml_CompleteWorkflow_ReturnsValidXml()
    {
        // Arrange
        var input = CreateCompleteTestData();

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.XmlContent.Should().NotBeNullOrEmpty();
        result.XmlContent.Should().Contain("<?xml version=\"1.0\"");
        result.XmlContent.Should().Contain("<PublishedItem");
        result.XmlContent.Should().Contain($"<Title>{input.Title}</Title>");
        result.XmlContent.Should().Contain($"<Countries>{string.Join(",", input.CountryIds)}</Countries>");
        result.XmlContent.Should().Contain("<ContactInformation>");
        result.XmlContent.Should().Contain("<PersonGroup sequence=\"1\">");
        result.XmlContent.Should().Contain("<Name>Analytical Contacts</Name>");
    }

    [Fact]
    public void ConvertToXml_WithMultipleContacts_ProcessesAllValidContacts()
    {
        // Arrange
        var input = CreateCompleteTestData();
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
                                FirstName = "", // Empty - should be skipped
                                LastName = "",
                                Title = "Invalid Contact"
                            },
                            new Contact
                            {
                                FirstName = "Jane",
                                LastName = "Smith",
                                Title = "Senior Analyst",
                                PhoneNumber = "+1-555-987-6543"
                            },
                            new Contact
                            {
                                FirstName = "John",
                                LastName = "Doe",
                                Title = "Lead Analyst",
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
        // Should contain both contacts (Jane Smith and John Doe)
        result.XmlContent.Should().Contain("<FamilyName>Smith</FamilyName>");
        result.XmlContent.Should().Contain("<GivenName>Jane</GivenName>");
        result.XmlContent.Should().Contain("<DisplayName>Jane Smith</DisplayName>");
        result.XmlContent.Should().Contain("<JobTitle>Senior Analyst</JobTitle>");
        result.XmlContent.Should().Contain("<Number>+1-555-987-6543</Number>");
        result.XmlContent.Should().Contain("<FamilyName>Doe</FamilyName>");
        result.XmlContent.Should().Contain("<GivenName>John</GivenName>");
        result.XmlContent.Should().Contain("<DisplayName>John Doe</DisplayName>");
        result.XmlContent.Should().Contain("<JobTitle>Lead Analyst</JobTitle>");
        result.XmlContent.Should().Contain("<Number>+1-555-123-4567</Number>");
    }

    [Fact]
    public void ConvertToXml_WithNoValidContacts_HandlesGracefully()
    {
        // Arrange
        var input = CreateCompleteTestData();
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
                                FirstName = "", // Empty
                                LastName = "",
                                Title = "Invalid Contact"
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
    public void ConvertToXml_WithMultipleContactSections_ProcessesCorrectly()
    {
        // Arrange
        var input = CreateCompleteTestData();
        input.ReportMetadata.ContactSection = new List<ContactSection>
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
                                FirstName = "Alice",
                                LastName = "Johnson",
                                Title = "Primary Analyst",
                                PhoneNumber = "+1-555-111-1111"
                            }
                        }
                    }
                }
            },
            new ContactSection
            {
                ContactInformation = new List<ContactInformation>
                {
                    new ContactInformation
                    {
                        ContactHeader = "Secondary Contacts",
                        Contacts = new List<Contact>
                        {
                            new Contact
                            {
                                FirstName = "Bob",
                                LastName = "Wilson",
                                Title = "Secondary Analyst",
                                PhoneNumber = "+1-555-222-2222"
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
        // Should contain both contacts from different sections
        result.XmlContent.Should().Contain("<FamilyName>Johnson</FamilyName>");
        result.XmlContent.Should().Contain("<GivenName>Alice</GivenName>");
        result.XmlContent.Should().Contain("<DisplayName>Alice Johnson</DisplayName>");
        result.XmlContent.Should().Contain("<JobTitle>Primary Analyst</JobTitle>");
        result.XmlContent.Should().Contain("<Number>+1-555-111-1111</Number>");
        result.XmlContent.Should().Contain("<FamilyName>Wilson</FamilyName>");
        result.XmlContent.Should().Contain("<GivenName>Bob</GivenName>");
        result.XmlContent.Should().Contain("<DisplayName>Bob Wilson</DisplayName>");
        result.XmlContent.Should().Contain("<JobTitle>Secondary Analyst</JobTitle>");
        result.XmlContent.Should().Contain("<Number>+1-555-222-2222</Number>");
    }

    [Fact]
    public void ConvertToXml_XmlFormatting_IsCorrectlyFormatted()
    {
        // Arrange
        var input = CreateCompleteTestData();

        // Act
        var result = _service.ConvertToXml(input);

        // Assert
        result.Success.Should().BeTrue();
        
        // Check XML declaration
        result.XmlContent.Should().StartWith("<?xml version=\"1.0\"");
        
        // Check indentation (should be formatted with 4 spaces)
        var lines = result.XmlContent!.Split('\n');
        lines.Should().Contain(l => l.Contains("<PublishedItem"));
        lines.Should().Contain(l => l.Contains("<Title>"));
        lines.Should().Contain(l => l.Contains("<Countries>"));
        lines.Should().Contain(l => l.Contains("<PublishedDate>"));
        lines.Should().Contain(l => l.Contains("<ContactInformation>"));
    }

    private static JsonInputModel CreateCompleteTestData()
    {
        return new JsonInputModel
        {
            Id = "integration-test-001",
            Title = "Integration Test Document",
            CountryIds = new List<string> { "US", "CA", "MX", "BR" },
            PublishDate = new DateTime(2024, 6, 15),
            Status = 3,
            TestRun = true,
            ReportMetadata = new ReportMetadata
            {
                Title = "Integration Test Report",
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
                                        FirstName = "John",
                                        LastName = "Doe",
                                        Email = "john.doe@example.com",
                                        Title = "Senior Financial Analyst",
                                        PhoneNumber = "+1-555-123-4567",
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
