namespace CarRental.Legacy;

public sealed class Renter
{
    public int Id { get; init; }

    public string Name { get; init; } = "";

    public string Email { get; init; } = "";

    public RenterKind Kind { get; init; }

    public int DoneCount { get; init; }

    public DateTime SinceUtc { get; init; }

    public bool HasValidEmail => Email is not null && Email.Contains("@");

    public bool IsLoyal => DoneCount > PricingRules.LoyalRentals;
}
