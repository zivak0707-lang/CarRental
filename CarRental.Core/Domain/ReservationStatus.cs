namespace CarRental.Core.Domain;

/// <summary>Стан бронювання в його життєвому циклі.</summary>
public enum ReservationStatus
{
    /// <summary>Чернетка: позиції ще можна додавати.</summary>
    Draft = 0,

    /// <summary>Бронювання підтверджено.</summary>
    Confirmed = 1,

    /// <summary>Автомобіль видано, оренда триває.</summary>
    Active = 2,

    /// <summary>Автомобіль повернуто, бронювання закрито.</summary>
    Closed = 3,

    /// <summary>Бронювання скасовано.</summary>
    Cancelled = 4,
}
