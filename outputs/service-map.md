# Service Map

## Services and roles

- `Gateway`
  - HTTP proxy for downstream services

- `Auth`
  - login and register use cases

- `Catalog`
  - product-facing service

- `Inventory`
  - stock source of truth
  - gRPC server for basket stock lookup

- `Basket`
  - Redis-backed basket storage
  - gRPC client for stock checks

- `Order`
  - simple order API placeholder

- `Notification`
  - consumes RabbitMQ events and stores notifications in memory

## Local ports

- `gateway` -> `http://localhost:8000`
- `auth` -> `http://localhost:8001`
- `catalog` -> `http://localhost:8002`
- `order` -> `http://localhost:8003`
- `inventory` -> `http://localhost:8004`
- `notification` -> `http://localhost:8005`

## Communication style

- HTTP REST-like endpoints for public service APIs
- gRPC for Basket -> Inventory synchronous stock lookup
- RabbitMQ for Inventory -> Notification asynchronous events

## Why the map matters

It is easier to understand the system when each service has a single clear responsibility:

- Basket stores the user's selection
- Inventory owns stock numbers
- Notification reacts to events
- Gateway forwards HTTP traffic
