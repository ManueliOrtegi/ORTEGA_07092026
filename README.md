# FileProcessor API

A simple ASP.NET Core Web API for uploading CSV files and generating processing reports.

## Prerequisites

- .NET 10 SDK
- Visual Studio 2026

## Build the Application

Restore packages:

```bash
dotnet restore
```

Build the solution:

```bash
dotnet build
```

## Run the Application

Start the API:

```bash
dotnet run --project src/FileProcessor.Api
```

By default, the API runs at:

```text
http://localhost:3000
```

## API Key

Configure the API key in `appsettings.Development.json`:

```json
{
  "ApiKey": {
    "Key": "local-test-key"
  }
}
```

Include the API key in requests:

```http
X-API-Key: local-test-key
```

## API Documentation

Once the application is running:

- OpenAPI JSON: `http://localhost:3000/openapi/spec.json`
- Scalar UI: `http://localhost:3000/scalar`
- Test Page: `http://localhost:3000/test`

## Test Page Preview

The application includes a built-in test page for testing API endpoints directly from the browser.

docs/images/test-page.png

Using the test page, you can:

- Set the API key
- Test API connectivity
- Upload CSV files
- View processing reports

## Expected API Output

### Upload CSV File

**Endpoint**

```http
POST /api/files/upload
```

**Sample Input**

```csv
Name,Age,Salary
John Smith,28,75000
Jane Doe,32,85000
Bob Johnson,45,95000
Alice Williams,29,65000
Charlie Brown,51,105000
```

**Successful Response**

```json
{
  "fileName": "employees.csv",
  "uploadedAt": "2024-01-15T14:22:30Z",
  "rowsProcessed": 5,
  "averageSalary": 85000,
  "highestSalary": 105000,
  "lowestSalary": 65000,
  "processingTimeMilliseconds": 18
}
```

### Get Processing Report

**Endpoint**

```http
GET /api/files/report
```

**Successful Response**

```json
{
  "totalFilesProcessed": 3,
  "averageRowsProcessed": 45.67,
  "files": [
    {
      "fileName": "employees.csv",
      "uploadedAt": "2024-01-15T14:22:30Z",
      "rowsProcessed": 50,
      "processingTimeMilliseconds": 18
    },
    {
      "fileName": "staff.csv",
      "uploadedAt": "2024-01-15T14:25:45Z",
      "rowsProcessed": 35,
      "processingTimeMilliseconds": 12
    }
  ]
}
```

### Error Response

```json
{
  "success": false,
  "message": "Invalid API Key."
}
```

## Docker

Build the image:

```bash
docker build -t fileprocessor-api .
```

Run the container:

```bash
docker run -p 8080:8080 -e ApiKey__Key=local-test-key fileprocessor-api
```

## Project Structure

```text
src/
├── FileProcessor.Api
├── FileProcessor.Application
├── FileProcessor.Domain
└── FileProcessor.Infrastructure
```

## Features

- Clean Architecture
- CQRS with MediatR
- API Key Authentication
- FluentValidation
- EF Core InMemory Database
- Global Exception Handling
- OpenAPI Documentation
- Docker Support

## License

MIT License.
`