using CarRental.Core.Abstractions;
using CarRental.Core.Domain;

namespace CarRental.Core.Storage;

/// <summary>Сховище бронювань у пам'яті (словник за номером).</summary>
public sealed class InMemoryReservationRepository : IReservationRepository
{
    private readonly Dictionary<int, Reservation> _items = new();

    /// <inheritdoc/>
    public void Add(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        if (!_items.TryAdd(reservation.Id, reservation))
        {
            throw new InvalidOperationException(
                $"Бронювання № {reservation.Id} уже збережено.");
        }
    }

    /// <inheritdoc/>
    public Reservation? GetById(int id) => _items.GetValueOrDefault(id);

    /// <inheritdoc/>
    public IReadOnlyList<Reservation> GetAll() => _items.Values.ToList();
}
