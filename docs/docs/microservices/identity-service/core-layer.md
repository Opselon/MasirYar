---
sidebar_position: 4
---

# Core Layer

This layer is the most central and independent part of the `IdentityService` microservice. It contains the domain entities and the core business rules that are used in all other parts of the application.

## Responsibilities

-   **Defining Domain Entities:** This layer includes the definition of the main domain entities, such as `User`. These entities have no dependency on external technologies and only contain their own data and logic.
-   **Defining Core Interfaces:** If needed, the main interfaces that are implemented by other layers (such as `IRepository`) can be defined in this layer to ensure that dependencies are directed inwards (towards `Core`).
-   **Complete Independence:** This layer has no dependency on other layers (such as `Application` or `Infrastructure`) and should not have any. This independence makes the core business logic portable and testable.

## Structure

-   `Entities`: This folder contains all the domain entities. Each entity is a POCO (Plain Old CLR Object) class that includes properties and, if necessary, methods for executing its own logic.

## Design Principles

-   **Domain-Driven Design (DDD):** This layer is built based on the principles of Domain-Driven Design (DDD), where the main focus is on accurately modeling the business domain.
-   **The Dependency Rule:** In accordance with Clean Architecture, all dependencies must be directed towards this layer. No code in `Core` should depend on code in the outer layers (such as `Infrastructure` or `Api`).
