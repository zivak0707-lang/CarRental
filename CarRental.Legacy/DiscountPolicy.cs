namespace CarRental.Legacy;

public sealed class DiscountPolicy
{
    public decimal For(decimal subtotal, Renter renter)
    {
        decimal rate = RateFor(renter);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private static decimal RateFor(Renter renter) => renter.Kind switch
    {
        RenterKind.Vip => PricingRules.VipRate,
        RenterKind.Staff => PricingRules.StaffRate,
        _ => renter.IsLoyal ? PricingRules.LoyalRate : 0m,
    };
}
