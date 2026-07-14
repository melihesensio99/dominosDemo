# OpsFlow AI Microservice Starter

This repository starts as a direct microservice setup with a small but real base.

## What is in the repo?

- API gateway
- Auth service
- Catalog service
- Basket service
- Order service
- Inventory service
- Notification service
- Frontend app
- `BuildingBlocks` shared project
- Redis
- RabbitMQ
- PostgreSQL

## Current architecture

- `src/BuildingBlocks` holds shared primitives
- `src/Services/Auth/Auth.Api` uses a vertical slice layout
- `src/Services/Auth/Auth.Api/Features` holds auth commands, handlers, and endpoints
- `src/Services/Auth/Auth.Api/Infrastructure` holds the in-memory repository and security helpers
- `src/Services/Catalog/Catalog.Api`, `src/Services/Order/Order.Api`, `src/Services/Inventory/Inventory.Api`, `src/Services/Notification/Notification.Api`, and `src/Services/Gateway/Gateway.Api` each expose one service API

The rest of the services currently start as simple minimal APIs so we can learn the flow before adding more layers.

## Services

- `gateway` on `http://localhost:8000`
- `auth` on `http://localhost:8001`
- `catalog` on `http://localhost:8002`
- `basket` on `http://localhost:8006`
- `order` on `http://localhost:8003`
- `inventory` on `http://localhost:8004`
- `notification` on `http://localhost:8005`
- `frontend` on `http://localhost:5173`

## Quick start

1. Build and run the stack:

```bash
docker compose up --build
```

2. Open the frontend:

```bash
http://localhost:5173
```

3. Check the gateway:

```bash
curl http://localhost:8000/health
```

4. Try the auth service:

```bash
curl -X POST http://localhost:8001/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"email\":\"admin@opsflow.ai\",\"password\":\"P@ssw0rd123\"}"
```

## MVP flow

1. Login through `auth`
2. List products through `catalog`
3. Add items to `basket`
4. Create an order through `order`
5. Check stock through `inventory`
6. Send a notification through `notification`

## Notes for you

- `Command` changes state
- `Query` reads state
- `Handler` contains the actual use-case logic
- `BuildingBlocks` is only for truly shared parts
- `Program.cs` stays thin and only wires things together
- vertical slice means feature folders instead of big layer folders

## Next steps

- Add database models for each service
- Add RabbitMQ events between order and notification
- Add Redis cache for catalog and stock reads
- Add Python RAG service later
