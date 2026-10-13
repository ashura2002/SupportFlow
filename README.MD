# SupportFlow — Help Desk & Ticketing API

A backend REST API for help desk and ticket management built with ASP.NET Core Web API. The system provides a foundation for managing support tickets, ticket assignments, replies, and user access while following Clean Architecture, CQRS, and Domain-Driven Design (DDD) principles. It also includes JWT authentication, role-based authorization, centralized error handling with Problem Details, and production error monitoring.

## Tech Stack

- **Backend:** .NET 10 (ASP.NET Core), C#, MediatR (CQRS), Entity Framework Core
- **Database:** PostgreSQL
- **Architecture:** Clean Architecture, Domain-Driven Design (DDD), Repository Pattern, Result Pattern, Unit of Work
- **Authentication:** JWT, Role-Based Authorization
- **File Storage:** Cloudinary — cloud-based file and image storage integration
- **Monitoring:** Sentry — error monitoring and exception tracking
- **Logging:** Serilog — structured application and HTTP request logging
- **Deployment:** Docker, Docker Compose, Render

## Key Features

- **Role-Based Access Control:** Supports role-based access to protected API operations.
- **Ticket Management:** Provides the foundation for creating and managing support tickets with ticket status, priority, category, requester, and agent assignment.
- **Ticket Assignment:** Supports assigning tickets to agents through explicit domain behavior.
- **Ticket Replies:** Models ticket replies to support communication between requesters and support personnel.
- **Secure Authentication:** JWT-based authentication with role-based authorization.
- **Input Validation:** Uses application-level validation to help reject invalid requests before executing use cases.
- **Domain-Driven Design:** Encapsulates business behavior and domain rules within domain entities and domain exceptions.
- **Centralized Error Handling:** Uses ASP.NET Core exception handling and standardized API error responses with problem details.
- **Structured Logging:** Uses Serilog for application and HTTP request logging.
- **Error Monitoring:** Integrates Sentry to capture application errors and support production troubleshooting.
- **Database Health Checks:** Provides a database health endpoint for checking database connectivity.
- **Containerization:** Uses Docker and Docker Compose to run the API and PostgreSQL database in a containerized environment.

## Architectural Highlights

The system follows **Clean Architecture** to separate business logic, application use cases, infrastructure implementations, and HTTP concerns.

- **Domain:** Entities, domain rules, domain exceptions, and business behavior.
- **Application:** CQRS Commands and Queries, request handlers, DTOs, validation, and application interfaces.
- **Infrastructure:** EF Core persistence, PostgreSQL integration, repositories, and external service integrations.
- **API:** RESTful endpoints, authentication, authorization, middleware, dependency injection, and HTTP request handling.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker Desktop with Docker Compose
- PostgreSQL, if running the API without Docker
- Cloudinary account and credentials, if file storage features are enabled
- Sentry project and DSN, if error monitoring is enabled

### Configuration

Configure the required application settings using environment variables or your local development configuration.

Typical settings include:

- Database connection string
- JWT signing configuration
- Seeded administrator account
- Cloudinary credentials
- Sentry DSN

Do not commit `.env` files, passwords, JWT secrets, API keys, or other sensitive credentials to version control.

### Run with Docker Compose

1. Clone the repository.
2. Configure the environment variables required by `docker-compose.yml`.
3. Start the application:

   ```bash
        docker compose up --build
   ```

4. Open the API documentation:

   ```text
        Open the Swagger documentation:

        http://localhost:8080/swagger

        Check database connectivity:

        http://localhost:8080/db-health
   ```

   For local development, use the Swagger URL configured by your Docker Compose setup.

5. Check database connectivity using the `/db-health` endpoint.

## Project Goals

SupportFlow is a portfolio project focused on building a maintainable, secure, and production-oriented backend API using ASP.NET Core. It emphasizes separation of concerns, explicit business rules, authentication and authorization, reliable persistence, and operational observability.

