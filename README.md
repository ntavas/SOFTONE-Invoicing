# SOFTONE-Invoicing

SOFTONE-Invoicing is a coding exercise for SOFTONE demonstrating a modern web API using best practices in software architecture. The project features a clean, layered design, robust error handling, secure API key authentication, and a comprehensive testing suite, making it a strong foundation for enterprise-grade applications.

### Key Features
* **Layered Architecture**: Separates concerns for maintainability (API, Application, Domain, Infrastructure).
* **Custom Error Handling**: Uses a `Result` pattern for predictable and consistent API responses.
* **B2B API Key Authentication**: Securely authenticates companies using a custom middleware.
* **Containerized**: Fully containerized with Docker for easy setup and deployment.
* **Comprehensive Testing**: Includes both unit and integration tests.

---

## Technology Stack 🛠️
* **.NET 8 (ASP.NET Core Web API)**: For building the robust Web API.
* **Entity Framework Core**: For data access and persistence.
* **PostgreSQL**: As the relational database.
* **xUnit**: For unit and integration testing.
* **Docker**: For containerization and easy deployment.

---

## Getting Started

Follow these instructions to get the project up and running on your local machine.

### Prerequisites
To run this application, you only need one tool installed:
* **Docker & Docker Compose**: The application is designed to run entirely in Docker containers. [Install Docker](https://docs.docker.com/get-docker/).

### Running with Docker
The entire application (API and database) can be started with a single command.

1.  **Clone the repository** (if you haven't already):
    ```bash
    git clone https://github.com/ntavas/SOFTONE-Invoicing
    cd SOFTONE-Invoicing
    ```

2.  **Start the application**:
    ```bash
    docker-compose up
    ```
    This command will build the Docker images, create the containers, and start the API and database. The API will be available at `http://localhost:8080`.

3.  **Access Swagger UI**:
    Once running, you can explore the API documentation and make requests interactively via Swagger UI:
    **[http://localhost:8080/swagger](http://localhost:8080/swagger)**

4.  **Stop the application**:
    To stop and remove the containers, press `Ctrl+C` in the terminal and then run:
    ```bash
    docker-compose down
    ```

---

## API Usage

### Authentication
The invoice-related endpoints (`/api/invoice/*`) are protected and require API key authentication.

1.  **Get an API Token**: The database is pre-populated with sample data. Use one of the tokens from the table below.

    | Company ID | API Token           |
    |------------|---------------------|
    | 1          | `token_company_001` |
    | 2          | `token_company_002` |
    | 3          | `token_company_003` |
    | ...        | ...                 |
    | 20         | `token_company_020` |

2.  **Provide the Token**: You must include an `Authorization` header with your requests in the format `Bearer {token}`.

    * **In Swagger UI**: Click the "Authorize" button and enter your token (e.g., `token_company_001`).
    * **In cURL/API Clients**: Add the header `-H "Authorization: Bearer token_company_001"`.

### Endpoints

The base URL for the API is `http://localhost:8080`.

| Method | Endpoint                                               | Description                                                |
| :---   |:-------------------------------------------------------|:-----------------------------------------------------------|
| `POST` | `/api/invoice/createInvoice`                           | Creates a new invoice for the authenticated company.       |
| `GET`  | `/api/invoice/sent`                                    | Lists invoices sent by the authenticated company.          |
| `GET`  | `/api/invoice/received`                                | Lists invoices received by the authenticated company.      |
| `GET`  | `/api/users`                                           | Lists users (this route does not require authentication).  |

**Query Parameters for `GET` endpoints:**
* `counterpartyCompanyId`: Filter invoices by the other company involved.
* `dateIssued`: Filter invoices by the exact issue date.
* `invoiceId`: Retrieve a specific invoice by its ID.

### Example cURL Commands

* **Create an invoice** (as Company 1 for Company 15):
    ```bash
    curl -X "POST" "http://localhost:8080/api/invoice/createInvoice" \
    -H "accept: */*" \
    -H "Authorization: Bearer token_company_001" \
    -H "Content-Type: application/json" \
    -d "{\"dateIssued\": \"2025-08-27\", \"netAmount\": 10000, \"vatAmount\": 500, \"description\": \"the description\", \"counterpartyCompanyId\": 15}"
    ```

* **List invoices sent by Company 1**:
    ```bash
    curl -X "GET" "http://localhost:8080/api/invoice/sent" \
    -H "accept: */*" \
    -H "Authorization: Bearer token_company_001"
    ```

* **List invoices received by Company 1**:
    ```bash
    curl -X "GET" "http://localhost:8080/api/invoice/received" \
    -H "accept: */*" \
    -H "Authorization: Bearer token_company_001"
    ```

---

## Architecture and Design

The solution follows a layered architecture to separate concerns and promote maintainability.

```
SOFTONE-Invoicing/
│
├── Invoicing.Api/            # ASP.NET Core Web API (entry point, controllers, middleware)
├── Invoicing.Application/    # Application layer (services, DTOs, validators, mappings)
├── Invoicing.Domain/         # Domain layer (entities, value objects, error/result handling)
├── Invoicing.Infrastructure/ # Infrastructure layer (data access, persistence, DI)
├── Invoicing.Tests/          # Unit and integration tests
├── docker-compose.yml        # Docker orchestration
├── Invoicing.sln             # Solution file
├── ops                       # operations
│   └─ pg-init/               # Database initialization scripts
└── sonar-status.png          # SonarQube analysis report
```

## Testing

The project includes both unit and integration tests to ensure code quality and reliability.

### Running Tests
To run all tests locally from the command line, you will need the [.NET SDK](https://dotnet.microsoft.com/download) installed. Navigate to the root directory and execute:
```bash
dotnet test
```
- Unit Tests: Located in Invoicing.Tests/Unit, these tests check individual components in isolation using mocks.

- Integration Tests: Located in Invoicing.Tests/Integration, these tests verify the interaction between different application components, including the database. They use a separate test database to avoid conflicts.

### Code Quality
SonarQube was used to check code quality, maintainability, and coverage. You can find the latest report at the **sonar-status.png**.