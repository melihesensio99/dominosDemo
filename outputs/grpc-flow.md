# gRPC Flow

## Why gRPC is here

gRPC is used when one service needs a fast, typed, synchronous answer from another service.

In this repo the important case is:

- `Basket` asks `Inventory` for stock information

## Flow

1. Basket receives a request to add or update an item.
2. Basket calls the generated gRPC client.
3. The client sends a `GetStockRequest` with `product_id`.
4. Inventory handles the request in `InventoryStockGrpcService`.
5. Inventory reads the stock from its repository.
6. Inventory returns `GetStockResponse`.
7. Basket maps the response into `StockSnapshot`.
8. Basket checks whether the quantity fits and then stores the basket in Redis.

## Contract ownership

- Inventory owns the contract because Inventory is the source of stock truth.
- Basket consumes that contract as a client.

## Files involved

- `src/Contracts/Inventory.Contracts/Protos/inventory_stock.proto`
- `src/Services/Inventory/Inventory.Api/GrpcServices/InventoryStockGrpcService.cs`
- `src/Services/Basket/Basket.Api/Infrastructure/InventoryGrpcStockClient.cs`
- `src/Services/Basket/Basket.Api/Infrastructure/GrpcErrorMapper.cs`

## What gRPC is not used for here

- it is not used for basket persistence
- it is not used for notifications
- it is not used for asynchronous event delivery

Those jobs are handled by Redis or RabbitMQ instead.
