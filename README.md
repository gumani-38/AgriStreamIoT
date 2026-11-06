# AgriStreamIoT

AgriStreamIoT is an ASP.NET Core MVC web application designed to support smart agriculture through cloud-enabled IoT solutions. The platform enables the collection, management, and visualization of agricultural data while integrating with Azure-based services for scalable processing and automation.

## Features

- ASP.NET Core MVC Architecture
- Entity Framework Core
- SQL Server Database Integration
- Razor Views
- Cloud-Ready Design
- Azure Function Integration
- IoT Data Processing
- Scalable and Maintainable Structure

## Technology Stack

### Backend
- ASP.NET Core MVC
- C#
- Entity Framework Core

### Frontend
- Razor Views
- HTML5
- CSS3
- Bootstrap
- JavaScript

### Database
- SQL Server
- Entity Framework Core Migrations

### Cloud Services
- Microsoft Azure
- Azure Functions

## Azure Function Integration

This project integrates with an Azure Function application that handles serverless processing and cloud-based operations.

### Azure Function Repository

```text
https://github.com/gumani-38/AgristreamAzureFunction
```

The Azure Function project can be used for:

- Processing IoT sensor data
- Background task execution
- Cloud event handling
- Monitoring and automation workflows
- Scalable serverless computing

## Getting Started

### Prerequisites

- Visual Studio 2022
- .NET SDK
- SQL Server
- Azure Account (Optional)
- Azure Functions Core Tools (Optional)

### Clone the Repository

```bash
git clone https://github.com/gumani-38/AgriStreamIoT.git
```

### Navigate to the Project

```bash
cd AgriStreamIoT
```

### Configure Database

Update the connection string in:

```json
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=AgriStreamIoTDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Apply Migrations

```powershell
Update-Database
```

or

```bash
dotnet ef database update
```

### Run the Application

```bash
dotnet restore
dotnet build
dotnet run
```


