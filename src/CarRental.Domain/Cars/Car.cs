namespace CarRental.Domain.Cars;

/// <summary>Автомобіль парку філії; ідентифікується державним номером.</summary>
public sealed class Car
{
    /// <summary>Створює автомобіль.</summary>
    /// <param name="plate">Державний номер у верхньому регістрі, наприклад AA1024KB.</param>
    /// <param name="model">Марка й модель для показу клієнту.</param>
    /// <param name="carClass">Клас, за яким береться тариф.</param>
    public Car(string plate, string model, CarClass carClass)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plate);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);
        Plate = plate;
        Model = model;
        Class = carClass;
    }

    /// <summary>Державний номер, унікальний у парку.</summary>
    public string Plate { get; }

    /// <summary>Марка й модель.</summary>
    public string Model { get; }

    /// <summary>Клас автомобіля.</summary>
    public CarClass Class { get; }
}
