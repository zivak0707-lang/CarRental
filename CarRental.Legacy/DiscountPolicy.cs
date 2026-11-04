namespace CarRental.Legacy;

public sealed class DiscountPolicy
{
    // Порядок важливий: перше правило, що підходить, визначає ставку
    private readonly IReadOnlyList<IDiscountRule> _rules = new IDiscountRule[]
    {
        new VipDiscountRule(),
        new StaffDiscountRule(),
        new LoyalDiscountRule(),
    };

    public decimal For(decimal subtotal, Renter renter)
    {
        decimal rate = RateFor(renter);
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private decimal RateFor(Renter renter)
    {
        foreach (IDiscountRule rule in _rules)
        {
            decimal? rate = rule.RateFor(renter);
            if (rate.HasValue)
            {
                return rate.Value;
            }
        }

        return 0m;
    }
}
