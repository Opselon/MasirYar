---
sidebar_position: 3
---

# Application Layer

This layer is the heart of the business logic for the `IdentityService` microservice. It is responsible for coordinating operations, executing business rules, and managing the flow of data between the `Api` layer and the `Infrastructure` layer.

## Responsibilities

-   **Implementing Use Cases:** This layer includes the implementation of all system use cases, such as user registration, user login, etc.
-   **Executing Business Rules:** All business rules and logic are implemented in this layer to ensure data correctness and integrity.
-   **Coordinating Operations:** This layer acts as a coordinator and uses services from the `Infrastructure` layer to perform data-related operations and other external dependencies.
-   **No Dependency on Implementation Details:** This layer has no direct dependency on how technical details such as the database or external services are implemented and communicates with them through interfaces.

## Structure

-   `UseCases`: This folder contains all the system's use cases, grouped by features. Each feature includes its own `Command` or `Query` and the corresponding `Handler`.
    -   **Commands:** For operations that change the state of the system (such as creating, updating, or deleting data).
    -   **Queries:** For operations that read data from the system without changing its state.
    -   **Handlers:** Contain the main logic for processing `Commands` and `Queries`.
-   `Interfaces`: This folder contains interfaces that are implemented by the `Infrastructure` layer, such as `IUserRepository` and `IPasswordHasher`. These interfaces allow the `Application` layer to communicate with other layers without being dependent on their implementation details.

## Design Pattern

This layer utilizes the **CQRS (Command Query Responsibility Segregation)** pattern with the help of the `MediatR` library. This pattern helps to separate read and write operations, resulting in cleaner, more maintainable, and scalable code.
