using CarRental.Application.Abstractions;
using CarRental.Domain.Rentals;

namespace CarRental.Infrastructure.Json;

/// <summary>Сховище оренд у rentals.json; файл читається один раз і пишеться цілком.</summary>
public sealed class JsonRentalRepository : IRentalRepository
{
    /// <summary>Номер першого договору в порожньому сховищі.</summary>
    public const int FirstNumber = 1001;

    private readonly string _path;
    private readonly List<Rental> _items;

    /// <summary>Завантажує оренди з файлу; відсутній файл означає порожнє сховище.</summary>
    /// <param name="path">Шлях до rentals.json.</param>
    /// <exception cref="InvalidDataException">Файл пошкоджено.</exception>
    public JsonRentalRepository(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _path = path;
        _items = File.Exists(path)
            ? JsonFiles.ReadList<RentalRecord>(path).Select(RentalMapper.ToDomain).ToList()
            : [];
    }

    /// <inheritdoc/>
    public IReadOnlyList<Rental> All() => _items.OrderBy(r => r.Number).ToList();

    /// <inheritdoc/>
    public Rental? Find(int number) => _items.Find(r => r.Number == number);

    /// <inheritdoc/>
    public int NextNumber() => _items.Count == 0 ? FirstNumber : _items.Max(r => r.Number) + 1;

    /// <inheritdoc/>
    public void Add(Rental rental)
    {
        ArgumentNullException.ThrowIfNull(rental);
        _items.Add(rental);
    }

    /// <inheritdoc/>
    public void Save() => JsonFiles.WriteList(_path, _items.Select(RentalMapper.ToRecord));
}
