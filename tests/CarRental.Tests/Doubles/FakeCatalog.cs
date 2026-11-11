using CarRental.Application.Abstractions;
using CarRental.Domain.Cars;
using CarRental.Domain.Customers;
using CarRental.Domain.Tariffs;

namespace CarRental.Tests.Doubles;

/// <summary>Тестовий дублер довідників: три авто, два клієнти, тарифи на всі класи.</summary>
public sealed class FakeCatalog : IFleetCatalog
{
    private readonly List<Customer> _customers =
    [
        new(1, "Петренко Олена", "DL000001"),
        new(2, "Коваленко Андрій", "DL000002"),
    ];

    public IReadOnlyList<Car> Cars { get; } =
    [
        new("AA1001AA", "Skoda Fabia", CarClass.Economy),
        new("AA1002AA", "Renault Clio", CarClass.Economy),
        new("AA2001BB", "Kia Sportage", CarClass.Suv),
    ];

    public Car? FindCar(string plate) => Cars.FirstOrDefault(c => c.Plate == plate);

    public Customer? FindCustomer(int id) => _customers.Find(c => c.Id == id);

    public Tariff TariffFor(CarClass carClass) =>
        new(carClass, carClass == CarClass.Suv ? 1800m : 900m);
}
