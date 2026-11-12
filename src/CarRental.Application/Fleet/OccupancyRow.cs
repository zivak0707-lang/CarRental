using CarRental.Domain.Cars;

namespace CarRental.Application.Fleet;

/// <summary>Рядок звіту «Зайнятість автопарку» для одного класу автомобілів.</summary>
/// <param name="Class">Клас; null у підсумковому рядку.</param>
/// <param name="Total">Автомобілів у парку.</param>
/// <param name="Rented">Видано клієнтам.</param>
/// <param name="Reserved">Заброньовано на дату звіту.</param>
public sealed record OccupancyRow(CarClass? Class, int Total, int Rented, int Reserved)
{
    /// <summary>Вільних автомобілів.</summary>
    public int Free => Total - Rented - Reserved;

    /// <summary>Зайнятість, %, з одним знаком після коми; 0 для класу без авто.</summary>
    public decimal Percent => Total == 0 ? 0m
        : Math.Round(100m * (Rented + Reserved) / Total, 1, MidpointRounding.AwayFromZero);
}
