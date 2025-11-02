---
sidebar_position: 1
---

# IdentityService

This microservice is responsible for all operations related to users, authentication, and access management.

## Project Structure

This project follows the Clean Architecture and is divided into the following layers:

- **Core**: Contains the domain entities, which have no dependency on any specific framework.
- **Application**: Contains the business logic and Use Cases.
- **Infrastructure**: Contains technical implementations such as database access, external services, etc.
- **Api**: The presentation layer that includes controllers and web configurations.

## Domain Model

The main model of this service is the `User` entity, which is defined in the `Core` layer and includes basic user information such as ID, username, email, and password hash.
