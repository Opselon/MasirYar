# Kubernetes Deployment for MasirYar Platform

This directory contains all the Kubernetes manifests required to deploy the MasirYar platform. The manifests are organized by service, with each service having its own subdirectory.

## Deployment Architecture

The platform is deployed to a dedicated namespace, `masiryar-production`, to ensure logical isolation of resources. Each microservice and frontend application is deployed as a separate `Deployment` with a corresponding `Service` for internal communication.

## Labeling Strategy

All resources in this cluster adhere to the standardized labeling strategy recommended by Kubernetes. This ensures that resources are easily identifiable and can be managed effectively. The following labels are used:

-   `app.kubernetes.io/name`: The name of the application (e.g., `identity-service`).
-   `app.kubernetes.io/instance`: A unique name identifying the instance of the application (e.g., `masiryar-identity-service`).
-   `app.kubernetes.io/version`: The version of the application, which will be injected by the CI/CD pipeline.
-   `app.kubernetes.io/component`: The component type (e.g., `backend`, `frontend`).
-   `app.kubernetes.io/part-of`: The name of the overall platform (`masiryar`).

## Applying the Manifests

To deploy or update the platform, apply all the manifests in this directory using the following command:

```bash
kubectl apply -f . -n masiryar-production
```

This command should be run from the `kubernetes` directory. It will declaratively create or update all the resources in the `masiryar-production` namespace.
