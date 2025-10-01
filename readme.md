# AvenueCodeKBRA - JSON to XML Conversion API

A RESTful API built with .NET 8 that converts JSON input to well-formatted XML output with validation and configuration support.

## What This Program Does

This application provides a web API endpoint that:
- Accepts JSON input containing document metadata and contact information
- Validates the input against business rules
- Converts the validated JSON to a properly formatted XML structure
- Returns either the XML content or detailed error messages

### Key Features
- **Input Validation**: Ensures data quality with configurable business rules
- **XML Generation**: Creates well-formatted XML with proper structure and indentation
- **Configuration Support**: Environment-based configuration for flexible deployment
- **Error Handling**: Comprehensive error messages for debugging
- **Swagger Documentation**: Built-in API documentation for testing

## Technologies Used

### Core Framework
- **.NET 8**: Latest LTS version of .NET for optimal performance
- **ASP.NET Core**: Web API framework for RESTful services
- **C# 12**: Modern C# language features

### Key Libraries & Packages
- **Microsoft.Extensions.Options**: Configuration management
- **Microsoft.Extensions.Logging**: Structured logging
- **Swashbuckle.AspNetCore**: Swagger/OpenAPI documentation
- **System.Text.Json**: JSON serialization/deserialization
- **System.Xml.Serialization**: XML generation

### Development Tools
- **Visual Studio 2022** / **VS Code**: IDE support
- **Postman**: API testing (collection included)
- **Git**: Version control

## Business Rules

The API validates input data against these rules:
1. **Status Validation**: Status must equal `3`
2. **Publish Date Validation**: Publish date must be on or after the configured cutoff date
3. **Test Run Validation**: Only processes production runs (`TestRun = false`)

## How to Run

### Prerequisites
- .NET 8 SDK installed on your machine
- Git (for cloning the repository)

### 1. Clone the Repository
```bash
git clone <repository-url>
cd AvenueCodeKBRA
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Application
```bash
dotnet build
```

### 4. Run the Application
```bash
dotnet run --project AvenueCodeKBRA
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`

### 5. Access Swagger Documentation
Once running, navigate to:
- `https://localhost:5001/swagger` (HTTPS)
- `http://localhost:5000/swagger` (HTTP)

## Configuration

### Environment Variables
You can override configuration values using environment variables:

```bash
# Windows - Cutoff Date
set ApplicationSettings__CutoffDate=2024-09-01T00:00:00Z

# Windows - XML Output Settings
set ApplicationSettings__XmlOutput__PersonGroupSequence=2
set ApplicationSettings__XmlOutput__PersonGroupName="Media Contacts"

# Linux/Mac - Cutoff Date
export ApplicationSettings__CutoffDate=2024-09-01T00:00:00Z

# Linux/Mac - XML Output Settings
export ApplicationSettings__XmlOutput__PersonGroupSequence=2
export ApplicationSettings__XmlOutput__PersonGroupName="Media Contacts"
```

### Configuration Files
Edit `appsettings.json` or `appsettings.Development.json`:

```json
{
  "ApplicationSettings": {
    "CutoffDate": "2024-08-24T00:00:00Z",
    "XmlOutput": {
      "PersonGroupSequence": 1,
      "PersonGroupName": "Analytical Contacts"
    }
  }
}
```

### Configuration Options

#### ApplicationSettings
- **CutoffDate**: The minimum date for publish date validation (ISO 8601 format)
- **XmlOutput**: Configuration for XML output formatting

#### XmlOutput Settings
- **PersonGroupSequence**: The sequence number for person groups in XML output (default: 1)
- **PersonGroupName**: The name for person groups in XML output (default: "Analytical Contacts")

## API Usage

### Endpoint
```
POST /api/XmlConversion/convert
```

### Request Body Example
```json
{
  "Id": "TCnWpDVD",
  "ReportMetadata": {
    "Title": "Document for Publication > Best Report Yet!!",
    "ContactSection": [
      {
        "ContactInformation": [
          {
            "ContactHeader": "Media Contact",
            "Contacts": [
              {
                "FirstName": "Mike",
                "LastName": "Johnsen",
                "Email": "mike.johnsen@kbra.com",
                "Title": "Director of Communications & Marketing",
                "PhoneNumber": "+1 646-731-1347",
                "Accreditation": "CM&AA"
              }
            ]
          }
        ]
      }
    ]
  },
  "CountryIds": ["US"],
  "Title": "Document for Publication > Best Report Yet!!",
  "PublishDate": "2024-08-26T18:19:59Z",
  "Status": 3,
  "TestRun": false
}
```

### Success Response
```xml
<?xml version="1.0" encoding="utf-8"?>
<PublishedItem>
  <Title>Document for Publication > Best Report Yet!!</Title>
  <Countries>US</Countries>
  <PublishedDate>2024-08-26T18:19:59</PublishedDate>
  <ContactInformation>
    <PersonGroup sequence="1">
      <Name>Analytical Contacts</Name>
      <PersonGroupMember>
        <Person>
          <FamilyName>Johnsen</FamilyName>
          <GivenName>Mike</GivenName>
          <DisplayName>Mike Johnsen</DisplayName>
          <JobTitle>Director of Communications & Marketing</JobTitle>
          <ContactInfo>
            <Phone>
              <Number>+1 646-731-1347</Number>
            </Phone>
          </ContactInfo>
        </Person>
      </PersonGroupMember>
    </PersonGroup>
  </ContactInformation>
</PublishedItem>
```

### Error Response
```json
{
  "success": false,
  "errorMessage": "Publish date must be on or after 2024-08-24",
  "xmlContent": null
}
```

## Testing

### Using Postman
1. Import the included `Postman_Collection.json`
2. Set the `base_url` variable to your API endpoint
3. Run the test scenarios included in the collection

### Sample Files
- `SampleInput.json`: Valid input example
- `SampleInput - Copia.json`: Alternative test case
- `SampleResult.xml`: Expected XML output

## Project Structure

```
AvenueCodeKBRA/
├── Controllers/
│   └── XmlConversionController.cs    # API endpoint controller
├── Models/
│   ├── ApplicationSettings.cs        # Configuration model
│   ├── ConversionRequest.cs          # Request/response models
│   ├── JsonInputModel.cs            # Input data model
│   ├── ValidationResult.cs          # Validation result model
│   └── XmlOutputModel.cs            # XML output model
├── Services/
│   ├── IXmlConversionService.cs     # Service interface
│   └── XmlConversionService.cs      # Business logic implementation
├── Program.cs                       # Application entry point
├── appsettings.json                 # Production configuration
├── appsettings.Development.json     # Development configuration
└── Postman_Collection.json         # API testing collection
```

## Development Notes

### Architecture
- **Clean Architecture**: Separation of concerns with Controllers, Services, and Models
- **Dependency Injection**: Proper DI container usage for testability
- **Configuration Pattern**: Options pattern for strongly-typed configuration
- **Error Handling**: Comprehensive exception handling with logging

### Performance Considerations
- Async/await pattern for scalable I/O operations
- Efficient XML serialization with proper formatting
- Structured logging for monitoring and debugging

## License

This project is part of a technical assessment for AvenueCode.

---

**Note**: This challenge is designed to be completed under 2 hours and demonstrates code structure, specification following, and best practices in .NET development.