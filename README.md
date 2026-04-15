## AspNetMicroservices

Minimal e-commerce microservices sample built with ASP.NET Core.

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