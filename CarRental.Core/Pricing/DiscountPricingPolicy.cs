using CarRental.Core.Abstractions;
using CarRental.Core.Domain;

namespace CarRental.Core.Pricing;

/// <summary>Знижка на кожну позицію з наперед заданою часткою.</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private const decimal MaxRate = 0.5m;

    private readonly decimal _rate;

    /// <summary>Створює політику зі знижкою.</summary>
    /// <param name="rate">Частка знижки від 0 до 0,5.</param>
    public DiscountPricingPolicy(decimal rate)
    {
        if (rate < 0m || rate > MaxRate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                rate,
                "Знижка має бути від 0 до 0,5.");
        }

        _rate = rate;
    }

    /// <inheritdoc/>
    public decimal PriceOf(ReservationDay line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - _rate), 2);
    }
}
