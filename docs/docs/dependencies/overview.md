---
sidebar_position: 1
---

# Key Dependencies

This project relies on several key NuGet packages to provide essential functionality. Understanding these packages is crucial for working with the codebase.

## .NET & ASP.NET Core
-   **Framework:** Built on .NET 9.0 and ASP.NET Core.
-   **Purpose:** Provides the fundamental runtime, web hosting, and API framework for building the microservice.

## Entity Framework Core
-   **Package:** `Microsoft.EntityFrameworkCore`
-   **Purpose:** The primary Object-Relational Mapper (ORM) used for data access. It allows developers to work with a database using .NET objects, abstracting away the underlying SQL.
-   **Driver:** `Npgsql.EntityFrameworkCore.PostgreSQL` is used to connect to the PostgreSQL database.

## MediatR
-   **Package:** `MediatR`
-   **Purpose:** Implements the mediator pattern to decouple in-process messaging. It is used to implement the CQRS (Command Query Responsibility Segregation) pattern in the `Application` layer, separating commands (writes) from queries (reads). This leads to cleaner, more focused handlers.

## Hangfire
-   **Package:** `Hangfire.Core`, `Hangfire.PostgreSql`
-   **Purpose:** A powerful and easy-to-use library for background job processing. It allows us to offload long-running, non-critical tasks (like sending emails or processing reports) to a separate process, preventing them from blocking the API response.

## Testcontainers
-   **Package:** `Testcontainers.PostgreSql`
-   **Purpose:** Used in integration tests to spin up ephemeral, lightweight Docker containers for dependencies like databases. This ensures that tests run in a clean, isolated, and realistic environment without needing a pre-installed database.

## xUnit & Moq
-   **Packages:** `xunit`, `Moq`
-   **Purpose:** `xUnit` is the testing framework used for writing and running both unit and integration tests. `Moq` is a popular and powerful mocking library used in unit tests to create mock objects for dependencies, allowing for isolated testing of specific units of code.
