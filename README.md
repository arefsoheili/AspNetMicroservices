## AspNetMicroservices

Minimal e-commerce microservices sample built with ASP.NET Core.

### Project structure

The solution lives under `src/` and is organized by **services** (each service is deployable independently) plus **building blocks** (shared contracts).

```text
AspNetMicroservices/
├─ README.md
└─ src/
   ├─ aspnetrun-microservices.sln
   ├─ docker-compose.yml
   ├─ docker-compose.override.yml
   ├─ BuildingBlocks/
   │  └─ EventBus.Messages/
   │     └─ Events/
   │        └─ BasketCheckoutIntegrationEvent.cs
   └─ Services/
      ├─ Catalog/
      │  └─ Catalog.Api
      ├─ Basket/
      │  └─ Basket.Api
      ├─ Discount/
      │  ├─ Discount.Api
      │  └─ Discount.Grpc
      └─ Ordering/
         ├─ Ordering.Api
         ├─ Ordering.Application
         ├─ Ordering.Domain
         └─ Ordering.infrastructure
```

### Architecture overview

- **Service ownership & data**: Each service owns its own data store (MongoDB for Catalog, Redis for Basket, Postgres for Discount). `Ordering` is modeled with a Clean Architecture project split.
- **Sync calls**:
  - **Basket → Discount.Grpc**: Basket calculates pricing/discounts by calling the gRPC service.
  - **Clients → \*.Api**: Each service exposes an HTTP API surface.
- **Async messaging (event-driven)**:
  - **Basket → RabbitMQ → Ordering**: Basket publishes `BasketCheckoutIntegrationEvent` and Ordering consumes it to create orders.
  - Event contracts are kept in `BuildingBlocks/EventBus.Messages` so the publisher and consumer share the same message schema.

### Services

- **Catalog** (`Services/Catalog/Catalog.Api`): Product catalog (MongoDB).
- **Basket** (`Services/Basket/Basket.Api`): Shopping basket (Redis) + calls **Discount gRPC** + publishes checkout event to RabbitMQ.
- **Discount**
  - **REST** (`Services/Discount/Discount.Api`)
  - **gRPC** (`Services/Discount/Discount.Grpc`) backed by Postgres.
- **Ordering** (`Services/Ordering/*`): Clean Architecture (Api/Application/Domain/Infrastructure), handles commands/queries via MediatR and consumes **BasketCheckoutIntegrationEvent** from RabbitMQ to create orders.

### Infrastructure (Docker)

Docker Compose files live in `src/`:

- **MongoDB** (`catalogdb`)
- **Redis** (`basketdb`)
- **Postgres** (`discountdb`) + **pgAdmin** (`pgadmin`)
- **Portainer** (`portainer`)
- **Catalog.Api**, **Basket.Api**, **Discount.Api**, **Discount.Grpc**

> Note: **RabbitMQ** and **Ordering.Api** are not currently included in the provided `docker-compose*.yml`.

### Run (local)

Prereqs: **Docker Desktop** + **.NET SDK**.

Start the infrastructure + core services:

```bash
cd src
docker compose up -d
```

Start RabbitMQ (needed for Basket → Ordering event flow):

```bash
docker run -d --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  rabbitmq:3-management
```

Run Ordering API:

```bash
dotnet run --project src/Services/Ordering/Ordering.Api/Ordering.Api.csproj
```

### Ports (from `src/docker-compose.override.yml`)

- **Catalog.Api**: `http://localhost:8000`
- **Basket.Api**: `http://localhost:8001`
- **Discount.Api**: `http://localhost:8002`
- **Discount.Grpc**: `http://localhost:8003`
- **pgAdmin**: `http://localhost:5050`
- **Portainer**: `http://localhost:9080`
- **RabbitMQ management** (when started via command above): `http://localhost:15672` (guest/guest)

### Ordering API endpoints

Base route: `api/v1/order`

- **GET** `/{id:int}`
- **GET** `/by-user/{userName}`
- **POST** `/` (checkout/create order)
- **PUT** `/` (update order)
- **DELETE** `/{id:int}`

### Notes

- **Ordering storage**: `Ordering.Api/appsettings.json` defaults to `UseInMemoryDatabase: true` for local dev.
- **Messaging**: Basket and Ordering use `EventBusSettings` (host/user/password) to connect to RabbitMQ.