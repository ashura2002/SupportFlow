# SupportFlow — Help Desk & Ticketing API

SupportFlow is a backend REST API for help desk and ticket management built with **ASP.NET Core Web API and .NET 10**. It provides a foundation for managing support tickets, assignments, replies, and user access while following Clean Architecture, CQRS, and Domain-Driven Design (DDD) principles.

The application includes JWT authentication, role-based authorization, centralized error handling with Problem Details, structured logging, and production error monitoring. It also uses **YARP as a reverse proxy and load balancer**, allowing requests to be distributed across multiple API instances running in Docker.

## Architecture

SupportFlow uses Docker Compose to orchestrate the Gateway, multiple API instances, and PostgreSQL.

```text
                     Client
                        |
                        v
               YARP Gateway :8080
                RoundRobin LB
                   /       \
                  v         v
             API Instance 1  API Instance 2
                :8080          :8080
                  \             /
                   \           /
                    v         v
                   PostgreSQL
```

- **Gateway:** Routes incoming API requests and distributes them across available API instances using YARP's RoundRobin load-balancing policy.
- **API instances:** Run independently in separate containers while sharing the same PostgreSQL database.
- **PostgreSQL:** Provides persistent storage for application data.
- **Health checks:** YARP supports active and passive destination health monitoring to help manage unhealthy API instances.

The Gateway is the public entry point for API requests. Internal services communicate over the Docker Compose network using service names rather than `localhost`.

## Tech Stack

- **Backend:** .NET 10, ASP.NET Core Web API, C#
- **Reverse Proxy & Load Balancing:** YARP
- **Database:** PostgreSQL
- **ORM & Data Access:** Entity Framework Core
- **Architecture:** Clean Architecture, DDD, CQRS, Repository Pattern, Result Pattern, Unit of Work
- **Request Handling:** MediatR
- **Authentication:** JWT and role-based authorization
- **File Storage:** Cloudinary
- **Logging:** Serilog
- **Error Monitoring:** Sentry
- **Containerization:** Docker and Docker Compose
- **Deployment:** Docker-compatible hosting platforms, including Render where configured

## Key Features

- **Ticket Management:** Supports ticket workflows involving status, priority, category, requester, and agent assignment.
- **Ticket Assignment:** Encapsulates assignment behavior within the domain.
- **Ticket Replies:** Supports communication between requesters and support personnel.
- **Secure Authentication:** JWT-based authentication and role-based authorization.
- **Input Validation:** Validates incoming requests before executing application use cases.
- **Domain-Driven Design:** Encapsulates business rules in domain entities and domain exceptions.
- **Centralized Error Handling:** Uses standardized API error responses with Problem Details.
- **Structured Logging:** Uses Serilog for application and HTTP request logging.
- **Error Monitoring:** Integrates Sentry for exception tracking and troubleshooting.
- **Database Health Checks:** Provides a database connectivity health endpoint for internal health monitoring.
- **Reverse Proxy and Load Balancing:** Uses YARP to route requests to multiple API instances.
- **Containerized Environment:** Uses Docker Compose to run the Gateway, API instances, and PostgreSQL together.

## Architectural Highlights

The application follows Clean Architecture to separate business logic, application use cases, infrastructure implementations, and HTTP concerns.

- **Domain:** Entities, business behavior, domain rules, and domain exceptions.
- **Application:** CQRS commands and queries, handlers, DTOs, validation, and application interfaces.
- **Infrastructure:** EF Core persistence, PostgreSQL integration, repositories, and external service integrations.
- **API / Presentation:** HTTP endpoints, authentication, authorization, middleware, and dependency injection.
- **Gateway:** YARP reverse proxy configuration, request routing, load balancing, and destination health monitoring.

## Getting Started

### Prerequisites

- Git
- Docker Desktop with Docker Compose, or Docker Engine with the Compose plugin
- Cloudinary credentials if file-storage features require them
- A Sentry DSN if error monitoring is enabled

You do not need to install the .NET SDK or PostgreSQL locally to run the application through Docker Compose.

### 1. Clone the repository

```bash
git clone <repository-url>
cd SupportFlow
```

Replace `<repository-url>` with your repository's actual Git URL.

### 2. Configure environment variables

Create a `.env` file in the repository root, alongside `compose.yaml`.

Example:

```dotenv
# PostgreSQL
POSTGRES_DB=supportflow
POSTGRES_USER=supportflow
POSTGRES_PASSWORD=replace-with-a-strong-password

# JWT
JWT_KEY=replace-with-a-long-random-secret

# Seeded user
SEEDED_USER_FIRST_NAME=Admin
SEEDED_USER_LAST_NAME=User
SEEDED_USER_EMAIL=admin@example.com
SEEDED_USER_PASSWORD=replace-with-a-strong-password

# Cloudinary
CLOUDINARY_CLOUD_NAME=
CLOUDINARY_API_KEY=
CLOUDINARY_API_SECRET=
CLOUDINARY_FOLDER=supportflow

# SLA
SLA_RESOLUTION_HOURS=24

# Sentry
SENTRY_DSN=
```

Use secure, unique credentials for your environment. Confirm the required environment variables against `compose.yaml` and your application configuration. Provide valid Cloudinary and Sentry credentials when the corresponding integrations are enabled.

**Never commit `.env` files, JWT secrets, database passwords, or API credentials to version control.**

### 3. Start SupportFlow with Docker Compose

Build the application images and start all services in the background:

```bash
docker compose up --build -d
```

Docker Compose starts the PostgreSQL database, two API instances, and the YARP Gateway.

Check the status of the services:

```bash
docker compose ps
```

View the application logs:

```bash
docker compose logs -f gateway api-1 api-2
```

To inspect database logs:

```bash
docker compose logs -f db
```

### 4. Send requests through the Gateway

Once the services are running, use the Gateway as the API entry point:

**Base URL**

```text
http://localhost:8080
```

For example, send an authentication request to your configured login endpoint:

```http
POST http://localhost:8080/api/Auth/login
Content-Type: application/json
```

Use the request body required by your authentication endpoint.

The Gateway forwards matching `/api/...` requests to the API instances using the configured RoundRobin policy. You do not need to address `api-1` or `api-2` directly from the host.

> **API documentation:** The current Gateway route matches `/api/{**catch-all}` only. Swagger is not automatically exposed through the Gateway unless a Swagger route is configured separately. Likewise, `/db-health` is used for health monitoring but is not exposed by that API route.

### 5. Verify load balancing and health monitoring

Inspect the Gateway logs while sending multiple API requests:

```bash
docker compose logs -f gateway
```

Check that requests are forwarded to both API instances. Depending on the health state and request behavior, YARP can exclude unhealthy destinations and reevaluate their availability.

You can also test an API instance failure in your local environment:

```bash
docker compose stop api-1
```

Continue sending requests through `http://localhost:8080` and observe the Gateway logs. After YARP detects the unhealthy destination, requests should continue through the remaining available instance when the route and health policies permit it.

Restore the instance:

```bash
docker compose start api-1
```

Monitor the logs to observe its recovery and return to service.

**Note:** Health-check detection and recovery are not necessarily instantaneous. Their timing depends on the configured health-check policies and reactivation intervals.

### 6. Stop the application

Stop and remove the running containers and Compose network:

```bash
docker compose down
```

The PostgreSQL data volume is retained by default, so the database data can persist across container recreation.

To remove the database volume as well:

```bash
docker compose down -v
```

**Warning:** This removes the named PostgreSQL volume and its persisted database data. Use it only when you intentionally want to reset the database.

## Project Goals

SupportFlow is a portfolio project focused on building a maintainable, secure, and production-oriented backend with ASP.NET Core. It emphasizes separation of concerns, explicit business rules, authentication and authorization, reliable persistence, and operational observability.

The Dockerized Gateway setup also provides hands-on experience with reverse proxies, load balancing, service discovery, container networking, and health monitoring.