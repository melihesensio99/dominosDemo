using Order.Api.Domain.Events;

namespace Order.Api.Domain;

public sealed class Order
{
    private readonly List<IDomainEvent> domainEvents = [];

    private Order()
    {
        Items = [];
    }

    public Order(
        string id,
        string customerId,
        List<OrderItem> items,
        Address shippingAddress,
        Address billingAddress,
        Payment payment,
        OrderStatus status,
        DateTimeOffset createdAt,
        string note,
        decimal totalPrice)
    {
        Id = id;
        CustomerId = customerId;
        Items = items;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        Payment = payment;
        Status = status;
        CreatedAt = createdAt;
        Note = note;
        TotalPrice = totalPrice;
    }

    public string Id { get; private set; } = string.Empty;

    public string CustomerId { get; private set; } = string.Empty;

    public List<OrderItem> Items { get; private set; }

    public Address ShippingAddress { get; private set; } = default!;

    public Address BillingAddress { get; private set; } = default!;

    public Payment Payment { get; private set; } = default!;

    public OrderStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public string Note { get; private set; } = string.Empty;

    public decimal TotalPrice { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.ToArray();

    public bool CanBeCancelled => Status is OrderStatus.Pending or OrderStatus.Confirmed;

    public static Order Create(
        string customerId,
        IEnumerable<OrderItem> items,
        Address shippingAddress,
        Address billingAddress,
        PaymentMethod paymentMethod,
        decimal totalPrice,
        string? note = null)
    {
        var orderItems = items.ToList();
        var order = new Order(
            Guid.NewGuid().ToString("N"),
            customerId,
            orderItems,
            shippingAddress,
            billingAddress,
            Payment.Create(paymentMethod),
            OrderStatus.Pending,
            DateTimeOffset.UtcNow,
            note?.Trim() ?? string.Empty,
            totalPrice);
        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, order.CustomerId, order.Items.Count));
        return order;
    }

    public bool Cancel()
    {
        return ChangeStatus(OrderStatus.Cancelled);
    }

    public bool ChangeStatus(OrderStatus newStatus)
    {
        if (!CanTransition(Status, newStatus))
        {
            return false;
        }

        var previousStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new OrderStatusChangedDomainEvent(
            Id,
            CustomerId,
            previousStatus,
            newStatus));

        if (newStatus == OrderStatus.Cancelled)
        {
            AddDomainEvent(new OrderCancelledDomainEvent(Id, CustomerId));
        }

        return true;
    }

    public void ClearDomainEvents() => domainEvents.Clear();

    public void MarkPaymentPaid()
    {
        Payment = Payment.MarkPaid();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkPaymentFailed()
    {
        Payment = Payment.MarkFailed();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private void AddDomainEvent(IDomainEvent domainEvent) => domainEvents.Add(domainEvent);

    private static bool CanTransition(OrderStatus currentStatus, OrderStatus newStatus) =>
        (currentStatus, newStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Preparing) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            (OrderStatus.Preparing, OrderStatus.Shipped) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            _ => false,
        };
}
