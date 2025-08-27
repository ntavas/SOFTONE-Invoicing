# SOFTONE-Invoicing

SOFTONE-Invoicing is a coding exercise for SOFTONE. The project follows best practices in architecture, error handling, dependency injection, and testing, providing a robust foundation for enterprise-grade applications.

---

## Project Structure

The solution follows a layered architecture to separate concerns and promote maintainability.

```
SOFTONE-Invoicing/
│
├── Invoicing.Api/           # ASP.NET Core Web API (entry point, controllers, middleware)
├── Invoicing.Application/   # Application layer (services, DTOs, validators, mappings)
├── Invoicing.Domain/        # Domain layer (entities, value objects, error/result handling)
├── Invoicing.Infrastructure/# Infrastructure layer (data access, persistence, DI)
├── Invoicing.Tests/         # Unit and integration tests
├── docker-compose.yml       # Docker orchestration
├── Invoicing.sln            # Solution file
├── ops                      # operations
│     └─ pg-init /           # Database initialization scripts
└── sonar-status.png         # SonarQube analysis report
```

### Layered Architecture

- **Invoicing.Api**: Hosts controllers, middleware, and API configuration. It serves as the entry point for all HTTP requests. It keeps startup logic clean by delegating service registrations to the Application and Infrastructure layers.
- **Invoicing.Application**: Contains the core business logic, including service interfaces and their implementations, Data Transfer Objects (DTOs), validators, and object-mapping logic.
- **Invoicing.Domain**: Represents the core of the application. It contains domain entities, value objects, and custom error and result handling classes. This layer is completely independent of any other layer.
- **Invoicing.Infrastructure**: Handles data access, persistence, and other external concerns. It provides concrete implementations for the repository interfaces defined in the Application layer.
- **Invoicing.Tests**: Contains unit and integration tests for the solution, ensuring code quality and correctness.

---

## Best Practices

### Error Handling

The project uses a custom error handling mechanism to ensure consistent and predictable error responses.

- **`Error.cs`**: Defines a standard structure for application errors, including a code and a message.
- **`Result.cs`**: A generic wrapper for service method return values. It can hold either a successful result or an error, preventing exceptions from bubbling up to the controllers.
- **`ResultToActionResultExtension.cs`**: An extension method that converts a `Result` object into an appropriate `IActionResult` (e.g., `OkObjectResult`, `BadRequestObjectResult`), simplifying controller logic.

### Dependency Injection

Dependency injection is used throughout the solution to promote loose coupling and testability.

- **`IServiceCollection` Extensions**: The `Invoicing.Api` project's `Program.cs` is kept clean by using extension methods to register services from other layers.
  - `AddApplication()`: Registers services from the `Invoicing.Application` layer.
  - `AddInfrastructure()`: Registers services from the `Invoicing.Infrastructure` layer.

### Authorization

Authorization is handled via a custom middleware for B2B API key authentication.

- **`ApiKeyAuthMiddleware.cs`**: This middleware intercepts requests to `/api/invoice/*` endpoints.
  - It expects an `Authorization` header in the format `Bearer {token}`.
  - The provided token is hashed using SHA-256 and compared against the `api_token_hash` stored for each company in the database.
  - If authentication is successful, the corresponding `CompanyId` is attached to the `HttpContext` for use in controllers and services.
  - If authentication fails, a `401 Unauthorized` or `403 Forbidden` response is returned with a consistent error format.

---

## Getting Started

### Running the Project

The project is containerized using Docker for easy setup and deployment.

1.  **Start the application**:
    ```bash
    docker-compose up
    ```
    This command will build the Docker images and start the necessary containers (API and database).

2.  **Stop the application**:
    ```bash
    docker-compose down
    ```
    This command will stop and remove the containers.

### API Endpoints
Base path: http://localhost:8080

- **Swagger UI**: Once the application is running, you can access the Swagger UI for interactive API documentation at:
  `http://localhost:8080/swagger/index.html`


- **POST** /api/invoice/createInvoice
> Create invoice (issuer = authenticated company).

- GET /api/invoice/sent?counterpartyCompanyId=&dateIssued=&invoiceId=
>List invoices sent by the authenticated company.

- GET /api/invoice/received?counterpartyCompanyId=&dateIssued=&invoiceId=
> List invoices received by the authenticated company.

- GET /api/users
> List users (no auth middleware on this route).

  To access these endpoints, you must provide a valid API token in the `Authorization` header.
  - **Header**: `Authorization: Bearer {token}`
  - **Example Token**: `token_company_001`

---

## Testing

The project includes both unit and integration tests to ensure the reliability of the codebase.

### Running Tests
- **Run all tests**:
    ```bash
    dotnet test
    ```

For sonar analysis, you can check the `sonar-status.png` image in the root of the repository.
### Unit Testing

Unit tests are focused on testing individual components in isolation. They are located in the `Invoicing.Tests/Unit` directory and use mocking to isolate dependencies.

### Integration Testing

Integration tests are designed to test the interaction between different components of the application, including the database. They are located in the `Invoicing.Tests/Integration` directory. These tests use a separate test database to ensure that they do not interfere with development data.

### Example Data 
For testing purposes, the database is pre-populated with example data for companies and invoices. You can use the following API tokens to authenticate requests:

| Company ID | API Token         |
|------------|-------------------|
| 1          | token_company_001 |
| 2          | token_company_002 |
| 3          | token_company_003 |
| ...        | ...               |
| 20         | token_company_020 |
This allows you to test the API endpoints with different companies and their associated invoices.

For better testing open the **Swagger UI** and use the "Authorize" button to input the API token.
 
Below are also some example cURL commands to test the API endpoints directly from the command line.
### Example cURL Commands
- Create Invoice
```bash
curl -X "POST" "http://localhost:8080/api/invoice/createInvoice" -H "accept: */*" -H "Authorization: Bearer token_company_001" -H "Content-Type: application/json" -d "{\"dateIssued\": \"2025-08-27\", \"netAmount\": 10000, \"vatAmount\": 500, \"description\": \"the description\", \"counterpartyCompanyId\": 15}"

curl -X "POST" "http://localhost:8080/api/invoice/createInvoice" -H "accept: */*" -H "Authorization: Bearer token_company_009" -H "Content-Type: application/json" -d "{\"dateIssued\": \"2025-08-27\", \"netAmount\": 3, \"vatAmount\": 3, \"description\": \"Another description\", \"counterpartyCompanyId\": 1}"
```

- Get Invoices sent by company with id 1
```bash
curl -X "GET" "http://localhost:8080/api/invoice/sent" -H "accept: */*" -H "Authorization: Bearer token_company_001"
```

- Get Invoices received by company for company with id 1
```bash
curl -X "GET" "http://localhost:8080/api/invoice/received" -H "accept: */*" -H "Authorization: Bearer token_company_001"
```
