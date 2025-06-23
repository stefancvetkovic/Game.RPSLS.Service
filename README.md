# RPSLS Game Service

Rock, Paper, Scissors, Lizard, Spock (RPSLS) Game Service - A .NET 8 Web API implementing the extended version of the classic Rock-Paper-Scissors game.

## Game Rules

- **Rock** crushes **Lizard** and **Scissors**
- **Paper** covers **Rock** and disproves **Spock**
- **Scissors** cuts **Paper** and decapitates **Lizard**
- **Lizard** eats **Paper** and poisons **Spock**
- **Spock** vaporizes **Rock** and smashes **Scissors**

## Architecture

This project follows **Clean Architecture** principles with **Domain-Driven Design (DDD)**, organized in an onion architecture pattern:

### Layer Structure

- **`1.Domain`** - Core business logic, entities, value objects, and domain services
- **`2.Application`** - CQRS implementation, DTOs, validators, and application logic
- **`3.Infrastructure`** - External concerns (caching, external APIs, data persistence)
- **`4.Presentation`** - RESTful API controllers, middleware, and HTTP concerns
- **`5.Tests`** - Comprehensive test suite including unit, integration, and architecture tests

### Key Patterns Implemented

- **CQRS** (Command Query Responsibility Segregation) using MediatR
- **Repository Pattern** for data access abstraction
- **Domain Services** for complex business logic
- **Value Objects** for domain concepts
- **Circuit Breaker Pattern** for external service resilience
- **Middleware Pipeline** for cross-cutting concerns

## Prerequisites

- **.NET 8.0 SDK** or later
- **Docker** (optional, for containerized deployment)
- **Git** for version control

### Installing .NET 8

Download and install the .NET 8 SDK from [Microsoft's official website](https://dotnet.microsoft.com/download/dotnet/8.0).

Verify installation:
```bash
dotnet --version
```

## Project Setup

### 1. Clone the Repository

```bash
git clone https://github.com/stefancvetkovic/Game.RPSLS.Service
cd RPSLS.Service
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run Tests

```bash
# Run all tests
dotnet test

# Run specific test projects
dotnet test src/5.Tests/RpslsGameService.Api.Tests
dotnet test src/5.Tests/RpslsGameService.Application.Tests
dotnet test src/5.Tests/RpslsGameService.Domain.Tests
dotnet test src/5.Tests/RpslsGameService.Infrastructure.Tests
```

### 5. Run the Application

```bash
# Development mode
dotnet run --project src/4.Presentation/RpslsGameService.Api

# Or use the IDE launch profile
dotnet run --project src/4.Presentation/RpslsGameService.Api --launch-profile "RpslsGameService.Api"
```

The API will be available at:
- **HTTPS**: `https://localhost:56167`
- **HTTP**: `http://localhost:56168`

## Docker Setup

### Building the Docker Image

```bash
# Build the Docker image
docker build -f src/4.Presentation/RpslsGameService.Api/Dockerfile -t rpsls-game-service .
```

### Running with Docker

```bash
# Run the container
docker run -p 56167:56167 -p 56168:56168 rpsls-game-service
```

The containerized application will be available at:
- **HTTPS**: `https://localhost:56167`
- **HTTP**: `http://localhost:56168`

### Docker Compose (Optional)

Create a `docker-compose.yml` file for easier container management:

```yaml
version: '3.8'
services:
  rpsls-api:
    build:
      context: .
      dockerfile: src/4.Presentation/RpslsGameService.Api/Dockerfile
    ports:
      - "56167:56167"
      - "56168:56168"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=https://+:56167;http://+:56168
```

## Authentication & API Keys

The service supports two authentication methods:

### 1. API Key Authentication (Optional)
To enable API key authentication, update `appsettings.json`:

```json
{
  "ApiKeySettings": {
    "Enabled": true,
    "HeaderName": "X-API-Key",
    "ValidApiKeys": [
      "test123",
      "test321"
    ]
  }
}
```

## API Documentation

Once the application is running, access the interactive API documentation:

- **Swagger UI**: `https://localhost:56167`
- **OpenAPI Spec**: `https://localhost:56167/swagger/v1/swagger.json`

### Main Endpoints

- **`GET /api/game/choices`** - Get all available choices
- **`GET /api/game/choice`** - Get a random choice
- **`POST /api/game/play`** - Play a round
- **`GET /api/game/history`** - Get game statistics
- **`POST /api/game/reset`** - Reset scores
- **`GET /health`** - Health check

##  Configuration

### Application Settings

Key configuration sections in `appsettings.json`:

```json
{
  "ApplicationSettings": {
    "HttpsPort": 56167,
    "HttpPort": 56168
  },
  "ExternalApis": {
    "RandomNumberService": {
      "BaseUrl": "https://codechallenge.boohma.com",
      "TimeoutSeconds": 30,
      "CircuitBreaker": {
        "OpenTimeoutSeconds": 30,
        "MinimumThroughput": 10,
        "SamplingPeriodSeconds": 60,
        "FailureRateThreshold": 50.0
      }
    }
  },
  "JwtSettings": {
    "Issuer": "RpslsGameService",
    "SecretKey": "your-secret-key-here",
    "ExpirationMinutes": 60
  }
}
```

## Logging

The application uses **Serilog** for structured logging:

- **File Output**: Logs saved to `logs/rpsls-game-*.txt`
- **Log Rotation**: Daily rolling files

## Testing Strategy

The project includes comprehensive tests:

### Test Types

1. **Unit Tests** - Test individual components in isolation
2. **Domain Tests** - Test business logic and domain models

## Development Tools

### Recommended IDE Extensions

- **Visual Studio 2022** or **Visual Studio Code**
- **C# Extensions**
- **REST Client** or **Postman** for API testing

## Health Monitoring

### Health Checks

The application includes health checks at `/health`:

```bash
curl https://localhost:56167/health
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Follow the existing code style and architecture
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## Troubleshooting

### Common Issues

1. **Port Conflicts**:
   - Ensure ports 56167 and 56168 are available
   - Check for other applications using these ports

2. **SSL Certificate Issues**:
   - Trust the development certificate: `dotnet dev-certs https --trust`

3. **External Service Failures**:
   - Check network connectivity to `https://codechallenge.boohma.com`
   - Review circuit breaker settings

4. **Build Errors**:
   - Ensure .NET 8 SDK is installed
   - Run `dotnet restore` to restore packages
   - Check for missing dependencies

### Log Troubleshooting

Check the logs in the `logs/` folder for detailed error information:

## License
Open MIT


## Acknowledgments

- Rock Paper Scissors Lizard Spock game rules by Sam Kass
- Clean Architecture principles by Robert C. Martin
- External random number service provided by [Code Challenge API](https://codechallenge.boohma.com/)
