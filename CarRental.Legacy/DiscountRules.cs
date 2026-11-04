namespace CarRental.Legacy;

public interface IDiscountRule
{
    decimal? RateFor(Renter renter);
}

public sealed class VipDiscountRule : IDiscountRule
{
    public decimal? RateFor(Renter renter) =>
        renter.Kind == RenterKind.Vip ? PricingRules.VipRate : null;
}

public sealed class StaffDiscountRule : IDiscountRule
{
    public decimal? RateFor(Renter renter) =>
        renter.Kind == RenterKind.Staff ? PricingRules.StaffRate : null;
}

public sealed class LoyalDiscountRule : IDiscountRule
{
    public decimal? RateFor(Renter renter) =>
        renter.IsLoyal ? PricingRules.LoyalRate : null;
}
