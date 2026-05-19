# Transaction Service
# TransactionService

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)

## Project Structure

| Project | Purpose                                                           |
  |---|-------------------------------------------------------------------|
| `TransactionService` | ASP.NET Core Web API                                              |
| `TransactionService.Domain` | Domain logic, data access (Dapper + Npgsql), DB migrations (DbUp) |
| `TransactionService.Test` | Acceptance tests (Testcontainers, Respawn)                        |
| `TransactionService.Domain.Test` | Unit tests                                                        |

## Running Locally

### Option 1 — Docker Compose (full stack)

  ```bash
  docker compose up --build
  ```
  API: http://localhost:8080

  Option 2 — dotnet run + Postgres only

  ```bash
  docker compose up postgres -d

  export DB_HOST=localhost DB_PORT=5432 DB_NAME=transactions DB_USER=postgres DB_PASSWORD=postgres
  dotnet run --project TransactionService/TransactionService.csproj
  ```

  API: http://localhost:5297

  Database

  Configured via environment variables. Migrations run automatically on startup via DbUp from TransactionService/Scripts/.

| Variable | Value |
  |---|---|
| `DB_HOST` | `postgres` (compose) / `localhost` (local) |
| `DB_PORT` | `5432` |
| `DB_NAME` | `transactions` |
| `DB_USER` | `postgres` |
| `DB_PASSWORD` | `postgres` |

  Tests

  Integration tests use Testcontainers — Docker must be running, no external DB setup needed.

  dotnet test                                                                 # all
  dotnet test TransactionService.Test/TransactionService.Test.csproj         # acceptance tests 
  dotnet test TransactionService.Domain.Test/TransactionService.Domain.Test.csproj  # unit

  ## API

  A TransactionService.http file is included in the TransactionService/ project for quick manual testing in your IDE. 
  ```
