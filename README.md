# NegotiationSystemAPI

NegotiationSystemAPI is a .NET 6 Web API for managing negotiations, proposals, and payments. The application uses Entity Framework Core with a code-first approach and migrations to generate the database schema. Windows Authentication is implemented for secure user identification, and Swagger is included for API documentation and testing.

## Features

- **Entity Framework Core**: Code-first approach with migrations for database generation.
- **Windows Authentication**: Secures the API by identifying users based on their Windows credentials.
- **Swagger Integration**: Provides an interface for testing and documenting the API.
- **Negotiation System**: Manages proposals, payments, and party items.

## Prerequisites

- .NET 6 SDK
- SQL Server
- Visual Studio
- Git (to clone the repository)



## Setup and Installation
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/karim-djk/NegotiationSystemAPI.git
   cd NegotiationSystemAPI
   ```

2. **Configure the Database**:
   - Ensure a local SQL Server instance is running.
   - Optionally update the `appsettings.json` file in the project to point to your SQL Server instance.

   Example:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=YourServerName;Database=NegotiationSystem;Trusted_Connection=True;"
   }
   ```

3. **Run Migrations**:  
   Open the **Package Manager Console** in Visual Studio (via `Tools > NuGet Package Manager > Package Manager Console`) and apply migrations to generate the database with the following command:  
   ```powershell
   Update-Database
   ```
   OR
   
   Use the .NET command-line interface (CLI):
   ```powershell
   dotnet ef database update
   ```
   
4. **Run the Application**:  
   Start the application by pressing `F5` in Visual Studio or using the `dotnet run` command in the terminal.

5. **Access Swagger UI**:  
   Once the application is running, navigate to the following URL in your browser:
   ```
   https://localhost:{port}/swagger
   ```
   Replace `{port}` with the port number shown in your terminal when the application starts (e.g., `https://localhost:5001/swagger`).  

   The Swagger UI provides:  
   - A visual representation of all available API endpoints.  
   - Interactive testing capabilities for each endpoint.  
   - Detailed information about request and response models.  
  
   You can use the Swagger UI to test endpoints like creating proposals, retrieving negotiations etc.
