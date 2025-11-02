---
sidebar_position: 2
---

# Api Layer

This layer is the main entry point for the `IdentityService` microservice and is responsible for managing incoming requests, validating them, and sending appropriate responses.

## Responsibilities

-   **Providing Endpoints:** This layer includes RESTful API controllers and gRPC services that make various operations such as user registration, login, and management available to clients.
-   **Authentication and Authorization:** This layer is responsible for implementing authentication and authorization policies to ensure that only authorized users have access to resources.
-   **Input Validation:** Input data from clients is validated in this layer to ensure its correctness and completeness.
-   **Serialization and Deserialization:** This layer is responsible for converting data from JSON format (for REST) or Protocol Buffers (for gRPC) to C# objects and vice versa.

## Structure

-   `Controllers`: Contains RESTful API controllers, each dedicated to a specific resource (e.g., `Users`).
-   `Services`: Contains the implementation of gRPC services.
-   `DTOs`: Contains Data Transfer Objects used for data exchange between the client and server.
-   `Extensions`: Contains helper methods for configuring services in `Program.cs`.
-   `Filters`: Contains custom filters for error handling and other cross-cutting concerns.
-   `Program.cs`: The starting point of the application, containing the main service configurations, such as Dependency Injection, Authentication, and the Middleware Pipeline.
