---
sidebar_position: 1
---

# Introduction

Welcome to the Personal Growth Platform! This project is a microservices-based application designed to help users track and improve their personal development.

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
