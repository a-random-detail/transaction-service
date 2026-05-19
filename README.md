# Transaction Service

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)

## Project Structure

| Project | Purpose                                                           |
  |---|-------------------------------------------------------------------|
| `TransactionService` | ASP.NET Core Web API                                              |
| `TransactionService.Domain` | Domain logic, data access (Dapper + Npgsql), DB migrations (DbUp) |
| `TransactionService.Test` | Acceptance/Functional tests (Testcontainers, Respawn)             |
| `TransactionService.Domain.Test` | Unit tests                                                        |

## Running Locally

### Option 1 — Docker Compose (full stack)

  ```bash
  docker compose up --build
  ```
  API: http://localhost:8080

  Option 2 — dotnet run + Postgres only

  ```bash
  docker compose up postgres redis -d

  export WRITE_DB_CONNSTRING="Host=localhost;Port=5432;Database=transactions;Username=postgres;Password=postgres"
  export READ_DB_CONNSTRING="Host=localhost;Port=5432;Database=transactions;Username=postgres;Password=postgres"
  export REDIS_CONNSTRING="localhost:6379"
  
  dotnet run --project TransactionService/TransactionService.csproj
  ```

  API: http://localhost:5297

  ### Infrastructure

  ```mermaid
  graph LR
      App --> Redis
      App --> WriteDB[Write DB]
      App --> ReadReplica[Read Replica]
  ```

  #### Database
  - Migrations run automatically on startup via DbUp from TransactionService/Scripts/. Migrations run against the write connection only —
  read replicas sync automatically (in production)

  | Variable | Value |
  |---|---|
  | `WRITE_DB_CONNSTRING` | `Host=postgres;Port=5432;Database=transactions;Username=postgres;Password=postgres` |
  | `READ_DB_CONNSTRING` | `Host=postgres;Port=5432;Database=transactions;Username=postgres;Password=postgres` |

  #### Cache

  | Variable | Value |
  |---|---|
  | `REDIS_CONNSTRING` | `redis:6379` |

  ### Tests

  Integration tests use Testcontainers — Docker must be running, no external DB setup needed.
  ```bash
  dotnet test                                                                 # all
  dotnet test TransactionService.Test/TransactionService.Test.csproj         # acceptance/functional tests 
  dotnet test TransactionService.Domain.Test/TransactionService.Domain.Test.csproj  # unit
  ```

  ## API

  A TransactionService.http file is included in the TransactionService/ project for quick manual testing in your IDE. 
  ```
