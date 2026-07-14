# Service Map

## Services and roles

- `Gateway`
  - HTTP proxy for downstream services

- `Auth`
  - login and register use cases

- `Frontend`
  - React storefront for browsing, basket, orders and notifications

- `Catalog`
  - product-facing service

- `Inventory`
  - stock source of truth
  - gRPC server for basket stock lookup

- `Basket`
  - Redis-backed basket storage
  - gRPC client for stock checks

- `Order`
  - PostgreSQL-backed order lifecycle API with CQRS handlers
  - persists order events through an outbox table
  - background worker publishes outbox messages to RabbitMQ

- `Notification`
  - consumes RabbitMQ stock and order events
  - stores notifications in MongoDB

## Local ports

- `gateway` -> `http://localhost:8000`
- `auth` -> `http://localhost:8001`
- `catalog` -> `http://localhost:8002`
- `basket` -> `http://localhost:8006`
- `order` -> `http://localhost:8003`
- `inventory` -> `http://localhost:8004`
- `notification` -> `http://localhost:8005`
- `frontend` -> `http://localhost:5173`

## Communication style

- HTTP REST-like endpoints for public service APIs
- gRPC for Basket -> Inventory synchronous stock lookup
- RabbitMQ for Inventory -> Notification stock events and Order -> Notification lifecycle events
- MongoDB for Notification persistence
- Outbox table for Order -> RabbitMQ dispatch

## Why the map matters

It is easier to understand the system when each service has a single clear responsibility:

- Basket stores the user's selection
- Inventory owns stock numbers
- Notification reacts to events
- Gateway forwards HTTP traffic
