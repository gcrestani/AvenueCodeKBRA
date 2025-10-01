# AvenueCodeKBRA Tests

This project contains unit tests for the AvenueCodeKBRA application using xUnit, Moq, and FluentAssertions.

## Test Structure

### Services Tests
- `XmlConversionServiceTests.cs` - Tests for the XML conversion service logic
  - Valid input conversion
  - Validation error handling
  - XML structure generation
  - Contact information processing

### Controller Tests
- `XmlConversionControllerTests.cs` - Tests for the API controller
  - JSON to XML conversion endpoint
  - File upload conversion endpoint
  - Error handling and response codes
  - Input validation

### Model Tests
- `ValidationResultTests.cs` - Tests for validation result model
- `ConversionResponseTests.cs` - Tests for conversion response model
- `JsonInputModelTests.cs` - Tests for JSON input model
- `XmlOutputModelTests.cs` - Tests for XML output models

### Integration Tests
- `XmlConversionIntegrationTests.cs` - End-to-end workflow tests
  - Complete conversion workflow
  - Multiple contact handling
  - XML formatting validation

## Running Tests

### Using .NET CLI
```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "ClassName=XmlConversionServiceTests"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Using Visual Studio
1. Open the solution in Visual Studio
2. Go to Test > Test Explorer
3. Build the solution
4. Run all tests or specific tests from the Test Explorer

## Test Dependencies

- **xUnit** - Testing framework
- **Moq** - Mocking framework for dependencies
- **FluentAssertions** - Fluent assertion library for readable tests
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing for ASP.NET Core

## Test Coverage

The tests cover:
- ✅ Service layer business logic
- ✅ Controller API endpoints
- ✅ Model validation and behavior
- ✅ Error handling scenarios
- ✅ Integration workflows
- ✅ Edge cases and boundary conditions

## Configuration

Test-specific configuration is available in `appsettings.Test.json` for any test-specific settings.
