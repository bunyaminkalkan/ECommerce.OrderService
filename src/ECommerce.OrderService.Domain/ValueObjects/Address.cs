namespace ECommerce.OrderService.Domain.ValueObjects;

public record Address
{
    public string Neighborhood { get; }
    public string Street { get; }
    public string BuildingNumber { get; }
    public string ApartmentNumber { get; }
    public string District { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    private Address(
        string neighborhood,
        string street,
        string buildingNumber,
        string apartmentNumber,
        string district,
        string city,
        string postalCode,
        string country = "Turkey")
    {
        Neighborhood = neighborhood ?? throw new ArgumentNullException(nameof(neighborhood));
        Street = street ?? throw new ArgumentNullException(nameof(street));
        BuildingNumber = buildingNumber ?? throw new ArgumentNullException(nameof(buildingNumber));
        ApartmentNumber = apartmentNumber ?? throw new ArgumentNullException(nameof(apartmentNumber));
        District = district ?? throw new ArgumentNullException(nameof(district));
        City = city ?? throw new ArgumentNullException(nameof(city));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
    }

    public static Address Create(
        string neighborhood,
        string street,
        string buildingNumber,
        string apartmentNumber,
        string district,
        string city,
        string postalCode,
        string country = "Turkey")
        => new(neighborhood, street, buildingNumber, apartmentNumber, district, city, postalCode, country);

    public string GetFullAddress()
        => $"{Neighborhood} Neighborhood, {Street} St., No: {BuildingNumber}, Apt: {ApartmentNumber}, {District}/{City}, {PostalCode}, {Country}";
}