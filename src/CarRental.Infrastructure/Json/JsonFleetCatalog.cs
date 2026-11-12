using CarRental.Application.Abstractions;
using CarRental.Domain.Cars;
using CarRental.Domain.Customers;
using CarRental.Domain.Tariffs;

namespace CarRental.Infrastructure.Json;

/// <summary>Довідники з файлів cars.json, customers.json і tariffs.json теки даних.</summary>
public sealed class JsonFleetCatalog : IFleetCatalog
{
    private readonly Dictionary<string, Car> _cars;
    private readonly Dictionary<int, Customer> _customers;
    private readonly Dictionary<CarClass, Tariff> _tariffs;

    /// <summary>Завантажує довідники; дані файлів перевіряються тут, на вході.</summary>
    /// <param name="dataDirectory">Тека з файлами довідників.</param>
    /// <exception cref="InvalidDataException">Файл пошкоджено або тарифу бракує.</exception>
    public JsonFleetCatalog(string dataDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
        _cars = JsonFiles.ReadList<CarRecord>(Path.Combine(dataDirectory, "cars.json"))
            .Select(c => new Car(c.Plate, c.Model, ParseClass(c.Class)))
            .ToDictionary(c => c.Plate, StringComparer.Ordinal);
        _customers = JsonFiles
            .ReadList<CustomerRecord>(Path.Combine(dataDirectory, "customers.json"))
            .Select(c => new Customer(c.Id, c.FullName, c.LicenseNumber))
            .ToDictionary(c => c.Id);
        _tariffs = JsonFiles.ReadList<TariffRecord>(Path.Combine(dataDirectory, "tariffs.json"))
            .Select(t => new Tariff(ParseClass(t.Class), t.DailyRate))
            .ToDictionary(t => t.Class);
        CarClass[] missing = Enum.GetValues<CarClass>()
            .Where(c => !_tariffs.ContainsKey(c))
            .ToArray();
        if (missing.Length > 0)
        {
            throw new InvalidDataException(
                $"У tariffs.json немає тарифу для: {string.Join(", ", missing)}.");
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<Car> Cars => _cars.Values.ToList();

    /// <inheritdoc/>
    public Car? FindCar(string plate) => _cars.GetValueOrDefault(plate);

    /// <inheritdoc/>
    public Customer? FindCustomer(int id) => _customers.GetValueOrDefault(id);

    /// <inheritdoc/>
    public Tariff TariffFor(CarClass carClass) => _tariffs[carClass];

    private static CarClass ParseClass(string name) =>
        Enum.TryParse(name, out CarClass value) && Enum.IsDefined(value)
            ? value
            : throw new InvalidDataException($"Невідомий клас автомобіля: «{name}».");
}
