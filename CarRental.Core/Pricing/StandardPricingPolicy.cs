using CarRental.Core.Abstractions;
using CarRental.Core.Domain;

namespace CarRental.Core.Pricing;

/// <summary>Звичайна ціна без знижок.</summary>
public sealed class StandardPricingPolicy : IPricingPolicy
{
    /// <inheritdoc/>
    public decimal PriceOf(ReservationDay line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return line.Amount;
    }
}
