# SupportFlow — Help Desk & Ticketing API

SupportFlow is a backend REST API for help desk and ticket management built with **ASP.NET Core Web API and .NET 10**. It provides a foundation for managing support tickets, assignments, replies, and user access while following Clean Architecture, CQRS, and Domain-Driven Design (DDD) principles.

The application includes JWT authentication, role-based authorization, centralized error handling with Problem Details, structured logging, and production error monitoring. **YARP** provides reverse-proxy routing and RoundRobin load balancing across multiple API destinations.

## Architecture

SupportFlow has two deployment setups: a local Docker Compose environment and a Render deployment with separately deployed services.

### Local development with Docker Compose

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
                       \          /
                        \        /
                         v      v
                     PostgreSQL
```

- **Gateway:** Routes incoming `/api/...` requests and distributes them across configured API destinations using YARP's RoundRobin policy.
- **API instances:** Run in separate containers and share the same PostgreSQL database.
- **PostgreSQL:** Provides persistent storage for application data.
- **Health checks:** YARP can monitor destinations using active and passive health checks when those checks are enabled and configured.
- **Docker networking:** The Gateway reaches the API containers through Docker Compose service names (`api-1` and `api-2`), not `localhost`.

### Render deployment

The deployed setup uses separate Render Web Services for the Gateway and the two WebAPI instances. The Gateway's public URL is the entry point to use when testing YARP routing.

```text
                         Client
                            |
                            v
                 Render Gateway Web Service
                         (YARP)
                     RoundRobin LB
                       /       \
                      v         v
             Render WebAPI 1   Render WebAPI 2
                      \         /
                       \       /
                        v     v
                       Neon PostgreSQL
```

Current WebAPI service URLs:

- **WebAPI 1:** `https://supportflow-fv1y.onrender.com/`
- **WebAPI 2:** `https://supportflow-api-2.onrender.com/`
- **Gateway:** Use the public URL shown on the deployed Gateway service in the Render Dashboard.

These URLs can change if the Render services are renamed or recreated. Update the Gateway destinations when a URL changes.

> **Render networking note:** The current setup uses the API services' HTTPS URLs as YARP destinations. Render Free Web Services cannot receive private-network traffic, so public HTTPS destinations can be useful for a free-tier portfolio deployment. If you move to a plan and topology that supports private networking, consider internal service addresses. Keep the API services authenticated and authorized; a public API URL can be called directly, bypassing Gateway-level policies.

## Tech Stack

- **Backend:** .NET 10, ASP.NET Core Web API, C#
- **Reverse Proxy & Load Balancing:** YARP
- **Database:** PostgreSQL (local Docker Compose) and Neon PostgreSQL (deployed environment)
- **ORM & Data Access:** Entity Framework Core
- **Architecture:** Clean Architecture, DDD, CQRS, Repository Pattern, Result Pattern, Unit of Work
- **Request Handling:** MediatR
- **Authentication:** JWT and role-based authorization
- **File Storage:** Cloudinary
- **Logging:** Serilog
- **Error Monitoring:** Sentry
- **Containerization:** Docker and Docker Compose
- **Deployment:** Render and Docker-compatible hosting platforms

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
- **Reverse Proxy and Load Balancing:** Uses YARP to route requests across multiple configured API destinations.
- **Containerized Environment:** Uses Docker Compose to run the Gateway, API instances, and PostgreSQL together locally.

## Architectural Highlights

The application follows Clean Architecture to separate business logic, application use cases, infrastructure implementations, and HTTP concerns.

- **Domain:** Entities, business behavior, domain rules, value objects, and domain exceptions.
- **Application:** CQRS commands and queries, handlers, DTOs, validation, and application interfaces.
- **Infrastructure:** EF Core persistence, PostgreSQL integration, repositories, and external service integrations.
- **API / Presentation:** HTTP endpoints, authentication, authorization, middleware, and dependency injection.
- **Gateway:** YARP reverse-proxy configuration, request routing, load balancing, and destination health monitoring.

## Getting Started — Local Docker Compose

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

Replace `<repository-url>` with the repository's actual Git URL.

### 2. Configure environment variables

Create a `.env` file in the repository root, alongside `compose.yaml` (or your Docker Compose file).

Example:

```dotenv
# PostgreSQL
POSTGRES_DB=supportflow
POSTGRES_USER=supportflow
POSTGRES_PASSWORD=replace-with-a-strong-password

# JWT
JWT_KEY=replace-with-a-long-random-secret

# Seeded platform administrator
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

Use secure, unique credentials for your environment. Confirm the required variables against the Compose file and application configuration. Provide valid Cloudinary and Sentry credentials when the corresponding integrations are enabled.

**Never commit `.env` files, JWT secrets, database passwords, or API credentials to version control.** ASP.NET Core User Secrets are for local development and are not automatically deployed to Render.

### 3. Start SupportFlow with Docker Compose

Build the application images and start all services in the background:

```bash
docker compose up --build -d
```

Docker Compose starts PostgreSQL, two API instances, and the YARP Gateway.

Check service status:

```bash
docker compose ps
```

View API and Gateway logs:

```bash
docker compose logs -f gateway api-1 api-2
```

View database logs:

```bash
docker compose logs -f db
```

### 4. Send requests through the Gateway

Once the services are running, use the Gateway as the API entry point.

**Local base URL**

```text
http://localhost:8080
```

Example login request:

```http
POST http://localhost:8080/api/Auth/login
Content-Type: application/json
```

Use the JSON request body required by the authentication endpoint. For other tests, prefer an existing endpoint that is safe to call repeatedly; avoid repeating state-changing requests unless that is intentional.

The Gateway forwards matching `/api/...` requests to configured API destinations. You do not need to address `api-1` or `api-2` directly from the host.

> **Routing note:** The current Gateway route matches `/api/{**catch-all}` only. Swagger is not automatically exposed through the Gateway unless a Swagger route is configured separately. The `/db-health` endpoint is used for health monitoring and is not covered by this API route.

### 5. Verify local load balancing and health monitoring

Inspect the Gateway logs while sending multiple requests:

```bash
docker compose logs -f gateway
```

Look for `Proxying to ...` messages and responses from both destinations. YARP can exclude unhealthy destinations and reevaluate their availability according to the configured health policies.

You can test destination failure locally:

```bash
docker compose stop api-1
```

Continue sending requests through `http://localhost:8080` and observe the Gateway logs. When YARP detects the destination as unhealthy, requests can continue through the remaining healthy instance, depending on the route and health configuration.

Restore the first instance:

```bash
docker compose start api-1
```

Health-check detection and recovery are not necessarily instantaneous; timing depends on the configured policies and intervals.

### 6. Stop the application

Stop and remove the containers and Compose network:

```bash
docker compose down
```

The PostgreSQL data volume is retained by default, so data can persist across container recreation.

To also remove the database volume:

```bash
docker compose down -v
```

**Warning:** This removes the named PostgreSQL volume and its persisted data. Use it only when intentionally resetting the local database.

## Deploying to Render

Deploy the Gateway and each WebAPI instance as separate Render Web Services. Use the same repository and branch, but configure the correct Dockerfile for each service.

### 1. Configure the WebAPI services

For both WebAPI services:

- **Runtime:** Docker
- **Dockerfile Path:** `Dockerfile` (repository root)
- Configure the service to listen on the port expected by Render. Render's default Web Service port is `10000`; the application must bind to `0.0.0.0` on the configured port.
- Configure the required application environment variables in that service's Render Dashboard.

The two API services should use the same Neon database and compatible application configuration so they operate on the same data and validate the same tokens.

Required environment-variable names include:

```text
ConnectionStrings__DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
Jwt__ExpiryInHours
SeededUser__SEEDED_USER_FIRST_NAME
SeededUser__SEEDED_USER_LAST_NAME
SeededUser__SEEDED_USER_EMAIL
SeededUser__SEEDED_USER_PASSWORD
Cloudinary__CloudName
Cloudinary__ApiKey
Cloudinary__ApiSecret
Cloudinary__Folder
Sla__ResolutionHours
SENTRY__DSN
```

Set the values appropriate to your deployment. Keep the Neon connection string, JWT key, seeded administrator password, and third-party credentials private. Do not copy actual secret values into this README or source control.

### 2. Configure the Gateway service

Create a separate Render Web Service using:

- **Runtime:** Docker
- **Dockerfile Path:** `Src/Presentation/Gateway/Dockerfile`
- **Docker Build Context:** repository root (`.`), if the Render setup presents this setting
- **Environment:** `ASPNETCORE_ENVIRONMENT=Production`
- **Port:** Bind to the configured Render port (default `10000`) on `0.0.0.0`.

Configure the YARP destination addresses in the **Gateway service's** environment settings. The keys must match the cluster and destination IDs in the Gateway's `appsettings.json`:

```text
ReverseProxy__Clusters__supportflow-cluster__Destinations__supportflow-api-1__Address=https://supportflow-fv1y.onrender.com/
ReverseProxy__Clusters__supportflow-cluster__Destinations__supportflow-api-2__Address=https://supportflow-api-2.onrender.com/
```

These are environment-variable examples for the currently deployed WebAPI URLs; update them if the services' URLs change. For Render Free services, public HTTPS destination URLs may be needed because Free Web Services cannot receive private-network traffic. If you use private networking on a supported plan, configure reachable internal addresses instead.

The Gateway does **not** need the WebAPI's database connection string or seeded-user credentials unless the Gateway itself contains code that uses those settings. Keep service-specific configuration with the service that needs it.

### 3. Test through the Gateway URL

Use the public URL shown for the **Gateway Web Service** in the Render Dashboard. Do not use a WebAPI URL when verifying that traffic passed through YARP.

Example:

```bash
curl -i https://<gateway-service>.onrender.com/api/<existing-endpoint>
```

Replace the placeholders with the Gateway's actual public URL and a real endpoint. Include a valid JWT header for protected endpoints.

Check the Gateway logs for messages such as:

```text
Proxying to https://supportflow-api-2.onrender.com/api/...
Received HTTP/2.0 response 200.

Proxying to https://supportflow-fv1y.onrender.com/api/...
Received HTTP/2.0 response 200.
```

These messages indicate that the Gateway forwarded requests to both destinations and received successful responses. For stronger verification, also inspect each API service's logs and confirm that both received requests.

### 4. Render Free-tier cold starts

Render Free Web Services spin down after 15 minutes without inbound traffic and typically take about a minute to start again when a new request arrives. During wake-up, the first request can be slow or encounter a temporary failure depending on timing and proxy behavior. In this project, a destination initially returned `502` while waking and returned `200` after it became available.

For repeatable load-balancing tests, warm up both destinations first and then send requests through the Gateway. Avoid interpreting a transient cold-start failure alone as proof that YARP routing is broken. See [Render's Free instance documentation](https://render.com/docs/free) for current limitations.

## Troubleshooting Notes

### `Unrecognized option '--connection'` when applying EF Core migrations

The installed `dotnet-ef` version may not support the `--connection` option. Instead, pass the database connection through the configuration source used by the startup project, such as an environment variable, and run `dotnet ef database update` without `--connection`.

For example, in PowerShell, for a configuration key of `ConnectionStrings:DefaultConnection`:

```powershell
$env:ConnectionStrings__DefaultConnection = "<your-neon-connection-string>"
dotnet ef database update --project <infrastructure-project-path> --startup-project <webapi-project-path> --context SupportFlowDbContext
```

Replace the project paths with the actual `.csproj` paths in your repository. Never commit the connection string or paste it into logs.

### `Email cannot be empty` during startup

The platform-admin seeder reads `SeededUserSettings`. Local User Secrets are not automatically available in Render. Configure the required variables on each WebAPI service, using keys that match the settings properties. The current settings class uses these exact keys:

```text
SeededUser__SEEDED_USER_FIRST_NAME
SeededUser__SEEDED_USER_LAST_NAME
SeededUser__SEEDED_USER_PASSWORD
SeededUser__SEEDED_USER_EMAIL
```

Configuration names must match the actual property names. For example, `SeededUser__Email` will not bind to `SEEDED_USER_EMAIL` unless the settings model uses a matching `Email` property. Do not log actual credentials; log only whether required values are configured.

### `Name or service not known (api-2:8080)` in YARP

`api-1` and `api-2` are Docker Compose service names. They resolve inside the local Compose network, not automatically across independent Render Web Services. For Render, configure the destination addresses using reachable URLs or supported internal addresses for the deployed services.

### `502` from a destination

Check the destination's direct URL, Render service health and deploy logs, application port binding, and any startup exceptions. If the destination is on a Free service that has been idle, allow it to wake and retry. A successful request to the direct API URL helps distinguish an API availability issue from a Gateway routing issue.

### Client IP appears as `::1` in Gateway logs

The Gateway has logged `[RateLimiter] Client IP: ::1` during testing. `::1` is the IPv6 loopback address. Before relying on per-client-IP rate-limiting behavior in deployment, verify how the application obtains `RemoteIpAddress` and configure forwarded headers only for trusted proxies. Do not blindly trust arbitrary forwarded-header values.

## Security and Operational Notes

- Never commit `.env` files, User Secrets, database credentials, JWT signing keys, or third-party API secrets.
- If a credential is exposed, rotate it and update the relevant deployment environment variables.
- Keep seeded administrator credentials different across environments and use a strong password.
- The current demo tenant seeder contains fixed demo-user credentials in code. Treat these as development/demo-only: do not enable demo seeding with predictable credentials in a production environment. Prefer explicit environment/configuration gating and secure per-environment credentials.
- If API services remain publicly reachable, clients may bypass Gateway-level policies by calling the APIs directly. Secure the API endpoints independently, and consider private services/networking where supported.
- Verify client-IP handling before relying on IP-based rate limiting behind a proxy.

## Project Goals

SupportFlow is a portfolio project focused on building a maintainable, secure, and production-oriented backend with ASP.NET Core. It emphasizes separation of concerns, explicit business rules, authentication and authorization, reliable persistence, and operational observability.

The Dockerized Gateway and deployed Render setup provide hands-on experience with reverse proxies, load balancing, service discovery, container networking, health monitoring, and production troubleshooting.
