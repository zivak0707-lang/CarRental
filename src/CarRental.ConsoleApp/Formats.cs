using System.Globalization;
using CarRental.Domain.Rentals;

namespace CarRental.ConsoleApp;

/// <summary>Форматування сум, дат і станів для виводу українською.</summary>
internal static class Formats
{
    private static readonly CultureInfo _uk = CultureInfo.GetCultureInfo("uk-UA");

    /// <summary>Сума у форматі «1 234,50 грн».</summary>
    /// <param name="amount">Сума, грн.</param>
    /// <returns>Текст суми.</returns>
    public static string Money(decimal amount) => amount.ToString("N2", _uk) + " грн";

    /// <summary>Момент у форматі «20.11.2026 10:00».</summary>
    /// <param name="moment">Момент.</param>
    /// <returns>Текст моменту.</returns>
    public static string Moment(DateTimeOffset moment) => moment.ToString("dd.MM.yyyy HH:mm", _uk);

    /// <summary>Число у форматі української культури.</summary>
    /// <param name="value">Число.</param>
    /// <returns>Текст числа.</returns>
    public static string Number(decimal value) => value.ToString("0.0", _uk);

    /// <summary>Назва стану для людини.</summary>
    /// <param name="status">Стан оренди.</param>
    /// <returns>Назва українською.</returns>
    public static string Status(RentalStatus status) => status switch
    {
        RentalStatus.Reserved => "заброньовано",
        RentalStatus.Active => "видано",
        RentalStatus.Closed => "закрито",
        _ => "скасовано",
    };
}
