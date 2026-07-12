# Lesson 01: Auth Service Terms

## Monorepo
One git repository that contains multiple projects.

## Microservice
An independently running service with its own deployment boundary.

## Clean Architecture
A style that keeps business logic away from infrastructure details.

## BuildingBlocks
Shared primitives that can be reused across services:

- `Result<T>`
- error models
- shared enums
- shared events

## CQRS
Separate command and query flows:

- command changes state
- query reads state

## MediatR
A library that routes requests to handlers so endpoints stay thin.

## Command
A request that changes state.

## Query
A request that only reads data.

## Handler
The class that executes the actual use case behind a command or query.

## Dependency Direction
Outer layers depend on inner layers, not the other way around.

## Hash
A one-way representation of sensitive data like passwords.

## Thin Program.cs
`Program.cs` should only wire services, middleware, and endpoints together.

## Vertical Slice Architecture
Code is grouped by feature instead of by technical layer.

- `Features/Login`
- `Features/Register`
- each feature can keep its command, handler, response, and endpoint together

## Auth Service Shape
For this repo, `Auth.Api` is the main service project and `Infrastructure` stays inside it as a helper folder.
