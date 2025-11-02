---
sidebar_position: 2
---

# High-Level Architecture

The platform is built using a microservices architecture, with each service responsible for a specific domain. The services communicate with each other using a combination of synchronous (gRPC) and asynchronous (RabbitMQ) communication.

## Microservices

*   **IdentityService:** Manages user authentication, registration, and user profiles.
*   **CoachingService:** Handles the core coaching and journaling features of the platform.

## Project Structure

The project is organized into the following directories:

*   `src`: Contains the source code for the microservices.
*   `docker`: Contains the `docker-compose.yml` file and other Docker-related files.
*   `protos`: Contains the Protocol Buffer definitions for gRPC communication.

Each microservice in the `src` directory follows a clean architecture pattern, with the following layers:

*   `Api`: The entry point for the service, containing the API controllers and gRPC services.
*   `Application`: Contains the business logic and use cases for the service.
*   `Core`: Contains the domain entities and interfaces.
*   `Infrastructure`: Contains the implementation of the interfaces defined in the `Core` layer, such as repositories and external service clients.
