# Architecture Notes

## Goal
This repository is a small but real microservice starter for learning service boundaries, clean architecture habits, CQRS, gRPC, RabbitMQ, and Redis.

## Main services

- `Auth`: login and registration flows
- `Catalog`: product listing and product features
- `Inventory`: stock management and gRPC stock lookup
- `Basket`: Redis-backed basket storage
- `Order`: simple order API placeholder
- `Notification`: RabbitMQ consumer for stock changes
- `Gateway`: HTTP reverse proxy for the services

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
- RabbitMQ between Inventory and Notification
- Redis for basket storage
