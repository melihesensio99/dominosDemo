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
        DateTimeOffset createdAt)
    {
        Id = id;
        CustomerId = customerId;
        Items = items;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        Payment = payment;
        Status = status;
        CreatedAt = createdAt;
    }

    public string Id { get; private set; } = string.Empty;

    public string CustomerId { get; private set; } = string.Empty;

    public List<OrderItem> Items { get; private set; }

    public Address ShippingAddress { get; private set; } = default!;

    public Address BillingAddress { get; private set; } = default!;

    public Payment Payment { get; private set; } = default!;

    public OrderStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.ToArray();

    public static Order Create(
        string customerId,
        IEnumerable<OrderItem> items,
        Address shippingAddress,
        Address billingAddress,
        PaymentMethod paymentMethod)
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
            DateTimeOffset.UtcNow);
        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, order.CustomerId, order.Items.Count));
        return order;
    }

    public bool Cancel()
    {
        if (Status == OrderStatus.Cancelled)
        {
            return false;
        }

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new OrderCancelledDomainEvent(Id, CustomerId));
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
}
