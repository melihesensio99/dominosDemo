# Architecture Notes

## Goal
This repository is a small but real microservice starter for learning service boundaries, clean architecture habits, CQRS, gRPC, RabbitMQ, and Redis.

## Main services

- `Auth`: login and registration flows
- `Catalog`: product listing and product features
- `Inventory`: stock management and gRPC stock lookup
- `Basket`: Redis-backed basket storage
- `Order`: PostgreSQL-backed order lifecycle API with CQRS handlers and outbox-based RabbitMQ dispatch
- `Notification`: RabbitMQ consumer for stock and order events with MongoDB storage
- `Gateway`: HTTP reverse proxy for the services
- `Frontend`: React + Vite storefront for the demo flow

## Shared projects

- `BuildingBlocks`
  - shared `Result` types
  - HTTP result mapping
  - validation behavior
  - global exception handling

- `Inventory.Contracts`
  - gRPC contract for stock lookup
  - protobuf-generated request and response types

## Design choices

- feature folders are preferred over big layer folders
- `Program.cs` stays thin
- business logic lives in handlers
- infrastructure details stay behind abstractions
- health endpoints were removed to keep the learning project clean

## Current learning focus

- CQRS through MediatR
- gRPC between Basket and Inventory
- RabbitMQ between Inventory and Notification, and between Order and Notification
- MongoDB for notification persistence
- Outbox pattern for Order event dispatch
- Redis for basket storage
- React frontend through the gateway
