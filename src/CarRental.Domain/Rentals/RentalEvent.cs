namespace CarRental.Domain.Rentals;

/// <summary>Подія, що змінює стан оренди.</summary>
public enum RentalEvent
{
    /// <summary>Видача автомобіля клієнту.</summary>
    PickUp = 0,

    /// <summary>Повернення автомобіля і розрахунок.</summary>
    Close = 1,

    /// <summary>Скасування бронювання.</summary>
    Cancel = 2,
}
