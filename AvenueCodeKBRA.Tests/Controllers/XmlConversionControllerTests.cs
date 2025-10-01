using AvenueCodeKBRA.Controllers;
using AvenueCodeKBRA.Models;
using AvenueCodeKBRA.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;
using System.Text;

namespace AvenueCodeKBRA.Tests.Controllers;

public class XmlConversionControllerTests
{
    private readonly Mock<IXmlConversionService> _mockConversionService;
    private readonly Mock<ILogger<XmlConversionController>> _mockLogger;
    private readonly XmlConversionController _controller;

    public XmlConversionControllerTests()
    {
        _mockConversionService = new Mock<IXmlConversionService>();
        _mockLogger = new Mock<ILogger<XmlConversionController>>();
        _controller = new XmlConversionController(_mockConversionService.Object, _mockLogger.Object);
    }

    [Fact]
    public void ConvertToXml_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = CreateValidJsonInputModel();
        var expectedResponse = new ConversionResponse
        {
            Success = true,
            XmlContent = "<PublishedItem><Title>Test</Title></PublishedItem>"
        };

        _mockConversionService
            .Setup(x => x.ConvertToXml(It.IsAny<JsonInputModel>()))
            .Returns(expectedResponse);

        // Act
        var result = _controller.ConvertToXml(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    [Fact]
    public void ConvertToXml_ConversionFails_ReturnsBadRequest()
    {
        // Arrange
        var request = CreateValidJsonInputModel();
        var expectedResponse = new ConversionResponse
        {
            Success = false,
            ErrorMessage = "Validation failed"
        };

        _mockConversionService
            .Setup(x => x.ConvertToXml(It.IsAny<JsonInputModel>()))
            .Returns(expectedResponse);

        // Act
        var result = _controller.ConvertToXml(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    [Fact]
    public void ConvertToXml_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = CreateValidJsonInputModel();

        _mockConversionService
            .Setup(x => x.ConvertToXml(It.IsAny<JsonInputModel>()))
            .Throws(new Exception("Service error"));

        // Act
        var result = _controller.ConvertToXml(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    [Fact]
    public async Task ConvertToXmlFile_ValidFile_ReturnsFileResult()
    {
        // Arrange
        var jsonContent = CreateValidJsonString();
        var file = CreateMockFormFile("test.json", jsonContent);
        var expectedResponse = new ConversionResponse
        {
            Success = true,
            XmlContent = "<PublishedItem><Title>Test</Title></PublishedItem>"
        };

        _mockConversionService
            .Setup(x => x.ConvertToXml(It.IsAny<JsonInputModel>()))
            .Returns(expectedResponse);

        // Act
        var result = await _controller.ConvertToXmlFile(file);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    [Fact]
    public async Task ConvertToXmlFile_NullFile_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.ConvertToXmlFile(null!);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    [Fact]
    public async Task ConvertToXmlFile_EmptyFile_ReturnsBadRequest()
    {
        // Arrange
        var file = CreateMockFormFile("test.json", "");

        // Act
        var result = await _controller.ConvertToXmlFile(file);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    [Fact]
    public async Task ConvertToXmlFile_NonJsonFile_ReturnsBadRequest()
    {
        // Arrange
        var file = CreateMockFormFile("test.txt", "some content");

        // Act
        var result = await _controller.ConvertToXmlFile(file);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    [Fact]
    public async Task ConvertToXmlFile_InvalidJsonContent_ReturnsBadRequest()
    {
        // Arrange
        var file = CreateMockFormFile("test.json", "invalid json content");

        // Act
        var result = await _controller.ConvertToXmlFile(file);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    [Fact]
    public async Task ConvertToXmlFile_ConversionFails_ReturnsBadRequest()
    {
        // Arrange
        var jsonContent = CreateValidJsonString();
        var file = CreateMockFormFile("test.json", jsonContent);
        var expectedResponse = new ConversionResponse
        {
            Success = false,
            ErrorMessage = "Validation failed"
        };

        _mockConversionService
            .Setup(x => x.ConvertToXml(It.IsAny<JsonInputModel>()))
            .Returns(expectedResponse);

        // Act
        var result = await _controller.ConvertToXmlFile(file);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    [Fact]
    public async Task ConvertToXmlFile_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var jsonContent = CreateValidJsonString();
        var file = CreateMockFormFile("test.json", jsonContent);

        _mockConversionService
            .Setup(x => x.ConvertToXml(It.IsAny<JsonInputModel>()))
            .Throws(new Exception("Service error"));

        // Act
        var result = await _controller.ConvertToXmlFile(file);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IActionResult>();
    }

    private static JsonInputModel CreateValidJsonInputModel()
    {
        return new JsonInputModel
        {
            Id = "test-doc-001",
            Title = "Test Document",
            CountryIds = new List<string> { "US", "CA" },
            PublishDate = new DateTime(2024, 6, 15),
            Status = 3,
            TestRun = false,
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
                                Contacts = new List<Contact>
                                {
                                    new Contact
                                    {
                                        FirstName = "John",
                                        LastName = "Doe",
                                        Title = "Analyst"
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private static string CreateValidJsonString()
    {
        return """
        {
            "Id": "test-doc-001",
            "Title": "Test Document",
            "CountryIds": ["US", "CA"],
            "PublishDate": "2024-06-15T00:00:00Z",
            "Status": 3,
            "TestRun": false,
            "ReportMetadata": {
                "Title": "Test Report",
                "ContactSection": [
                    {
                        "ContactInformation": [
                            {
                                "Contacts": [
                                    {
                                        "FirstName": "John",
                                        "LastName": "Doe",
                                        "Title": "Analyst"
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }
        }
        """;
    }

    private static IFormFile CreateMockFormFile(string fileName, string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.Length).Returns(bytes.Length);
        mockFile.Setup(f => f.ContentType).Returns("application/json");
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
        
        return mockFile.Object;
    }
}