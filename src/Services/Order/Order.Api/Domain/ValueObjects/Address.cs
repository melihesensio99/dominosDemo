namespace Order.Api.Domain;

public sealed record Address(
    string Street,
    string District,
    string City,
    string PostalCode,
    string Country)
{
    public static Address Create(
        string street,
        string district,
        string city,
        string postalCode,
        string country) =>
        new(
            street.Trim(),
            district.Trim(),
            city.Trim(),
            postalCode.Trim(),
            country.Trim());
}
