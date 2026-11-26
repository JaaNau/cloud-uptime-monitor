# Cloud Uptime Monitor

A cloud-focused DevOps portfolio project for monitoring the availability of HTTP endpoints.

The application allows HTTP endpoints to be registered and managed through a REST API. A background checker service will periodically test the configured endpoints, store response data, and update their availability status.

The main purpose of the project is to demonstrate practical software delivery, infrastructure automation, security scanning, and cloud deployment skills through a realistic application context.

> **Project status:** Active development. The Monitor API, PostgreSQL persistence, database migrations, automated tests, and backend CI pipeline are currently implemented.

## Current features

- ASP.NET Core REST API
- Monitor creation, retrieval, update, and deletion
- Input validation with Problem Details responses
- PostgreSQL persistence
- Entity Framework Core migrations
- Health endpoint for automated service checks
- Local PostgreSQL development environment with Docker Compose
- Persistent PostgreSQL storage through a Docker volume

## Architecture

Current implementation:

```text
HTTP client
    |
    v
ASP.NET Core Monitor API
    |
    | Entity Framework Core / Npgsql
    v
PostgreSQL
```

Planned architecture:

```text
Browser
    |
    v
React + TypeScript frontend
    |
    v
ASP.NET Core Monitor API
    |
    +-------------------- PostgreSQL
    ^
    |
.NET Checker Worker
    |
    v
Monitored HTTP endpoints

Later:

Monitor API
    |
    v
Notification Service
    |
    v
Discord webhook
```

## Technology stack

### Implemented

- C#
- .NET 10 LTS
- ASP.NET Core Minimal APIs
- Entity Framework Core
- Npgsql
- PostgreSQL 17
- Docker Compose
- GitHub Actions

### Planned

- TypeScript and React
- Docker container images
- GitHub Container Registry
- Trivy vulnerability scanning
- SBOM generation
- Terraform
- Ansible
- AWS EC2

## API

### Health

```http
GET /health
```

### Monitor management

```http
POST   /api/v1/monitors
GET    /api/v1/monitors
GET    /api/v1/monitors/{id}
PUT    /api/v1/monitors/{id}
DELETE /api/v1/monitors/{id}
```

## Development

The application currently supports local development with Docker Compose and PostgreSQL. Database configuration is provided through environment variables, and Entity Framework migrations keep the schema reproducible.

Detailed development and deployment instructions will be added as the containerized application and cloud workflows are completed.

## Roadmap

- [x] Monitor API health endpoint
- [x] Monitor CRUD operations
- [x] PostgreSQL persistence
- [x] Entity Framework migrations
- [x] Backend build and test workflow
- [x] NuGet dependency vulnerability audit
- [ ] Persist endpoint check results
- [ ] Implement monitor status transitions
- [ ] Build the .NET Checker Worker
- [ ] Add React and TypeScript dashboard
- [ ] Containerize all services
- [ ] Add Trivy image scanning and SBOM generation
- [ ] Publish versioned images to GitHub Container Registry
- [ ] Provision AWS infrastructure with Terraform
- [ ] Configure and deploy the application with Ansible
- [ ] Add on-demand deployment and infrastructure teardown workflows
- [ ] Add Discord outage and recovery notifications

## Planned cloud deployment

The AWS environment will be provisioned on demand rather than kept running continuously:

```text
GitHub Actions
    |
    v
Terraform provisions AWS infrastructure
    |
    v
Ansible configures the EC2 instance
    |
    v
Docker Compose starts the application
    |
    v
An external smoke test validates the deployment
    |
    v
Terraform destroys the environment after validation
```

This keeps the deployment reproducible while avoiding unnecessary cloud costs.

## License

This project is licensed under the MIT License. See LICENSE.