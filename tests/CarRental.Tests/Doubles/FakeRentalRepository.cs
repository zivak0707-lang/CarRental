using CarRental.Application.Abstractions;
using CarRental.Domain.Rentals;

namespace CarRental.Tests.Doubles;

/// <summary>Тестовий дублер сховища: оренди в пам'яті, лічильник збережень.</summary>
public sealed class FakeRentalRepository : IRentalRepository
{
    private readonly List<Rental> _items = [];

    public int SaveCount { get; private set; }

    public IReadOnlyList<Rental> All() => _items.OrderBy(r => r.Number).ToList();

    public Rental? Find(int number) => _items.Find(r => r.Number == number);

    public int NextNumber() => _items.Count == 0 ? 1001 : _items.Max(r => r.Number) + 1;

    public void Add(Rental rental) => _items.Add(rental);

    public void Save() => SaveCount++;
}
