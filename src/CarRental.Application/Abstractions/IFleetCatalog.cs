using CarRental.Domain.Cars;
using CarRental.Domain.Customers;
using CarRental.Domain.Tariffs;

namespace CarRental.Application.Abstractions;

/// <summary>Довідники філії: автопарк, клієнти, тарифи (лише читання).</summary>
public interface IFleetCatalog
{
    /// <summary>Усі автомобілі парку.</summary>
    IReadOnlyList<Car> Cars { get; }

    /// <summary>Шукає автомобіль за державним номером.</summary>
    /// <param name="plate">Номер у верхньому регістрі.</param>
    /// <returns>Автомобіль або null.</returns>
    Car? FindCar(string plate);

    /// <summary>Шукає клієнта за кодом.</summary>
    /// <param name="id">Код клієнта.</param>
    /// <returns>Клієнт або null.</returns>
    Customer? FindCustomer(int id);

    /// <summary>Тариф для класу автомобіля.</summary>
    /// <param name="carClass">Клас.</param>
    /// <returns>Тариф; є для кожного класу.</returns>
    Tariff TariffFor(CarClass carClass);
}
