using CarRental.Core.Abstractions;
using CarRental.Core.Domain;

namespace CarRental.Tests;

internal sealed class FakeReservationRepository : IReservationRepository
{
    private readonly Dictionary<int, Reservation> _items = new();

    public int AddCalls { get; private set; }

    public void Add(Reservation reservation)
    {
        _items[reservation.Id] = reservation;
        AddCalls++;
    }

    public Reservation? GetById(int id) => _items.GetValueOrDefault(id);

    public IReadOnlyList<Reservation> GetAll() => _items.Values.ToList();
}

internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price)
    {
        _price = price;
    }

    public decimal PriceOf(ReservationDay line) => _price;
}
