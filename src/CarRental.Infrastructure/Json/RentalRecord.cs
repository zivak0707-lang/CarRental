namespace CarRental.Infrastructure.Json;

/// <summary>Запис файлу rentals.json; окремий від домену, щоб домен не знав про формат.</summary>
internal sealed class RentalRecord
{
    /// <summary>Номер договору.</summary>
    public int Number { get; init; }

    /// <summary>Код клієнта.</summary>
    public int CustomerId { get; init; }

    /// <summary>Державний номер автомобіля.</summary>
    public string CarPlate { get; init; } = string.Empty;

    /// <summary>Запланований початок.</summary>
    public DateTimeOffset Start { get; init; }

    /// <summary>Запланований кінець.</summary>
    public DateTimeOffset End { get; init; }

    /// <summary>Ціна доби за договором.</summary>
    public decimal DailyRate { get; init; }

    /// <summary>Назва стану з переліку RentalStatus.</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>Фактичний момент видачі.</summary>
    public DateTimeOffset? PickedUpAt { get; init; }

    /// <summary>Фактичний момент повернення.</summary>
    public DateTimeOffset? ReturnedAt { get; init; }

    /// <summary>Оплачувані доби.</summary>
    public int? BillableDays { get; init; }

    /// <summary>Вартість діб.</summary>
    public decimal? RentCost { get; init; }

    /// <summary>Штраф за пізнє повернення.</summary>
    public decimal? LateFee { get; init; }
}
