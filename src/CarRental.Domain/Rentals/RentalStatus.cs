namespace CarRental.Domain.Rentals;

/// <summary>Стан документа оренди (таблиця переходів — docs/spec.md, розділ 5).</summary>
public enum RentalStatus
{
    /// <summary>Автомобіль заброньовано, ще не видано.</summary>
    Reserved = 0,

    /// <summary>Автомобіль видано клієнту.</summary>
    Active = 1,

    /// <summary>Автомобіль повернуто, суму нараховано.</summary>
    Closed = 2,

    /// <summary>Бронювання скасовано до видачі.</summary>
    Cancelled = 3,
}
