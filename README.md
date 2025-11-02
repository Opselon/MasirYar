# Personal Growth Platform - Microservices

This repository contains the source code for the Personal Growth Platform, a microservices-based application designed to help users track and improve their personal development.

## Architecture Overview

The platform is built using a microservices architecture, with each service responsible for a specific domain. The services communicate with each other using a combination of synchronous (gRPC) and asynchronous (RabbitMQ) communication.

### Microservices

*   **IdentityService:** Manages user authentication, registration, and user profiles.
*   **CoachingService:** Handles the core coaching and journaling features of the platform.

## Getting Started

To get the project up and running, you will need to have the following installed:

*   [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [Docker](https://www.docker.com/products/docker-desktop)
*   [Docker Compose](https://docs.docker.com/compose/install/)

### Running the Project

1.  **Clone the repository:**

    ```shell
    git clone https://github.com/your-username/personal-growth-platform.git
    cd personal-growth-platform
    ```

2.  **Run the project using Docker Compose:**

    ```shell
    docker-compose up -d --build
    ```

    This will build and run all the services, databases, and other infrastructure components in Docker containers.

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

## Contributing

Contributions are welcome! Please feel free to open an issue or submit a pull request.
