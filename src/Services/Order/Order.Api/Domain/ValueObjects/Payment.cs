namespace Order.Api.Domain;

public sealed record Payment(PaymentMethod Method, PaymentStatus Status)
{
    public static Payment Create(PaymentMethod method) => new(method, PaymentStatus.Pending);

    public Payment MarkPaid() => this with { Status = PaymentStatus.Paid };

    public Payment MarkFailed() => this with { Status = PaymentStatus.Failed };
}
