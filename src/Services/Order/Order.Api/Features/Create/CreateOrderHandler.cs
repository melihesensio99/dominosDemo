using BuildingBlocks.Common;
using System.Text.Json;
using Order.Api.Abstractions;
using Order.Api.Domain;
using Order.Api.Features.Shared;
using OrderItem = Order.Api.Domain.OrderItem;
using OrderEntity = Order.Api.Domain.Order;

namespace Order.Api.Features.Create;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    ICatalogInventoryClient catalogClient,
    IOrderStockClient stockClient) : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var detailsResult = await ResolveOrderDetailsAsync(request.Items, cancellationToken);
        if (!detailsResult.IsSuccess || detailsResult.Value is null)
        {
            return Result<OrderResponse>.Validation(
                detailsResult.Error?.Code ?? "order.invalid_stock_mapping",
                detailsResult.Error?.Message ?? "Order stock requirements could not be resolved.");
        }

        var order = OrderEntity.Create(
            request.CustomerId,
            request.Items.Select(item => new OrderItem(
                item.ProductId,
                item.Quantity,
                JsonSerializer.Serialize(item.SelectedOptionIds ?? []))),
            Address.Create(
                request.ShippingAddress.Street,
                request.ShippingAddress.District,
                request.ShippingAddress.City,
                request.ShippingAddress.PostalCode,
                request.ShippingAddress.Country),
            Address.Create(
                request.BillingAddress.Street,
                request.BillingAddress.District,
                request.BillingAddress.City,
                request.BillingAddress.PostalCode,
                request.BillingAddress.Country),
            request.PaymentMethod,
            detailsResult.Value.TotalPrice,
            request.Note);

        var reservation = await stockClient.ReserveAsync(order.Id, detailsResult.Value.Requirements, cancellationToken);
        if (!reservation.Success)
        {
            return Result<OrderResponse>.Conflict(
                reservation.ErrorCode ?? "inventory.reservation_failed",
                reservation.Message ?? "The requested stock could not be reserved.");
        }

        try
        {
            await orderRepository.SaveAsync(order, cancellationToken);
        }
        catch
        {
            await stockClient.ReleaseAsync(order.Id, CancellationToken.None);
            throw;
        }

        return Result<OrderResponse>.Success(OrderMapper.ToResponse(order));
    }

    private sealed record ResolveOrderDetailsResult(
        IReadOnlyCollection<OrderStockRequirement> Requirements,
        decimal TotalPrice);

    private async Task<Result<ResolveOrderDetailsResult>> ResolveOrderDetailsAsync(
        IEnumerable<CreateOrderItemRequest> orderItems,
        CancellationToken cancellationToken)
    {
        var requirements = new List<OrderStockRequirement>();
        decimal totalPrice = 0;

        foreach (var orderItem in orderItems)
        {
            var product = await catalogClient.GetProductAsync(orderItem.ProductId, cancellationToken);
            if (product is null || !product.IsActive)
            {
                return Result<ResolveOrderDetailsResult>.Validation(
                    "order.product_unavailable",
                    $"Product '{orderItem.ProductId}' is unavailable.");
            }

            var selectedOptionIds = orderItem.SelectedOptionIds?.ToHashSet() ?? [];
            var selectedOptions = product.Options
                .Where(option => selectedOptionIds.Contains(option.Id) && option.IsActive)
                .ToArray();
            if (selectedOptions.Length != selectedOptionIds.Count)
            {
                return Result<ResolveOrderDetailsResult>.Validation(
                    "order.invalid_product_options",
                    "One or more selected product options are invalid.");
            }

            // Calculate item price: (Base Price + Options Adjustments) * Quantity
            var unitPrice = product.Price + selectedOptions.Sum(option => option.PriceAdjustment);
            totalPrice += unitPrice * orderItem.Quantity;

            string? stockKey;
            if (product.InventoryTrackingType.Equals("direct", StringComparison.OrdinalIgnoreCase))
            {
                stockKey = product.InventoryKey ?? product.Id;
            }
            else
            {
                var optionStockKeys = selectedOptions
                    .Select(option => option.InventoryKey)
                    .Where(key => !string.IsNullOrWhiteSpace(key))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                stockKey = optionStockKeys.Length == 1 ? optionStockKeys[0] : null;
            }

            if (string.IsNullOrWhiteSpace(stockKey))
            {
                return Result<ResolveOrderDetailsResult>.Validation(
                    "order.stock_mapping_missing",
                    $"Product '{orderItem.ProductId}' is not connected to a stock item.");
            }

            requirements.Add(new OrderStockRequirement(stockKey, orderItem.Quantity));
        }

        var groupedRequirements = requirements
            .GroupBy(item => item.StockKey, StringComparer.OrdinalIgnoreCase)
            .Select(group => new OrderStockRequirement(group.Key, group.Sum(item => item.Quantity)))
            .ToArray();

        return Result<ResolveOrderDetailsResult>.Success(
            new ResolveOrderDetailsResult(groupedRequirements, totalPrice));
    }
}
