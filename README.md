# Personal Growth Platform - Microservices

This repository contains the source code for the Personal Growth Platform, a microservices-based application designed to help users track and improve their personal development.

## 🚀 Single-Command Setup

This project uses a **smart setup script** (`run.sh` for Linux/macOS and `run.ps1` for Windows) to fully manage the ecosystem. This script automates the entire process, from checking prerequisites to configuration and execution.

> [!IMPORTANT]
> **Prerequisite:** The only required tool is [Docker Desktop](https://www.docker.com/products/docker-desktop/).

### Getting Started

1.  **Clone the repository:**
    ```shell
    git clone https://github.com/Opselon/MasirYar.git
    cd MasirYar
    ```

2.  **Run the Setup Wizard:**
    Run the script for the first time. An interactive Wizard will guide you through creating the `.env` configuration file.

    **On Linux/macOS:**
    ```bash
    # Make the script executable (only once)
    chmod +x run.sh
    # Start all services
    ./run.sh up
    ```

    **On Windows (with PowerShell):**
    ```powershell
    # If needed, allow script execution for this session
    # Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
    # Start all services
    .\run.ps1 up
    ```

### Managing the Ecosystem

Use the setup script for all your daily tasks:

| Command                        | Description                                                        |
| ---------------------------- | --------------------------------------------------------------- |
| `up`                         | Start all services.                                       |
| `down`                       | Stop all services.                                      |
| `logs [service-name]`        | View live logs (example: `./run.sh logs identity-service`).     |
| `prune`                      | **(Dangerous)** Stop and completely remove everything, including data.         |
| `config`                     | Re-run the configuration Wizard.                                     |
| `test`                       | Run all automated tests for the project.                               |

> [!TIP]
> To see the full list of commands, run the script without any parameters (`./run.sh` or `.\run.ps1`).
