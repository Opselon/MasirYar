---
sidebar_position: 1
---

# Testing Strategy

This microservice has a complete set of automated tests to ensure the quality and stability of the code.

## Test Types

### 1. Unit Tests
- **Location:** `IdentityService.UnitTests`
- **Goal:** To test the pure and isolated business logic in the `Core` and `Application` layers (such as Command Handlers and domain entities).
- **Dependencies:** These tests have **no external dependencies**. The EF Core In-Memory Database is used to simulate the database.
- **Speed:** Very fast.
- **Execution:** `dotnet test`

### 2. Integration Tests
- **Location:** `IdentityService.IntegrationTests`
- **Goal:** To test the complete flow of a request from the API entry point (`Controller`) to the database, within a single microservice.
- **Dependencies:** These tests use **Testcontainers** to launch a **real** PostgreSQL database in a temporary and isolated Docker container for each test suite.
- **Speed:** Slower than unit tests, but very reliable.
- **Execution:** `dotnet test`
- **Note:** To run these tests, Docker must be installed and running on your system.

## How to Run All Tests

To run all the tests for this microservice, go to the root of the `IdentityService` folder and run the following command:

```shell
dotnet test
```

This command will automatically find and run all the test projects in the solution.
