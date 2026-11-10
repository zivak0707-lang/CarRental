namespace CarRental.Domain.Cars;

/// <summary>Клас автомобіля; від нього залежить тариф за добу.</summary>
public enum CarClass
{
    /// <summary>Економ: малолітражні міські авто.</summary>
    Economy = 0,

    /// <summary>Компакт: авто гольф-класу.</summary>
    Compact = 1,

    /// <summary>Позашляховик.</summary>
    Suv = 2,

    /// <summary>Преміум: бізнес-клас.</summary>
    Premium = 3,
}
