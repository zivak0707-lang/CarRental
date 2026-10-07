namespace CarRental.Core.Domain;

/// <summary>Стан прокату в його життєвому циклі.</summary>
public enum RentalStatus
{
    /// <summary>Новий прокат, ще не підтверджений.</summary>
    New = 0,

    /// <summary>Прокат підтверджено.</summary>
    Confirmed = 1,

    /// <summary>Автомобіль видано клієнтові, прокат триває.</summary>
    Active = 2,

    /// <summary>Прокат завершено.</summary>
    Completed = 3,

    /// <summary>Прокат скасовано.</summary>
    Cancelled = 4,
}
