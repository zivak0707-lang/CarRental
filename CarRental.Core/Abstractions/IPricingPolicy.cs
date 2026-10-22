using CarRental.Core.Domain;

namespace CarRental.Core.Abstractions;

/// <summary>Політика розрахунку ціни позиції бронювання.</summary>
public interface IPricingPolicy
{
    /// <summary>Рахує ціну позиції за правилами політики.</summary>
    /// <param name="line">Позиція бронювання.</param>
    /// <returns>Ціна позиції, грн.</returns>
    decimal PriceOf(ReservationDay line);
}
