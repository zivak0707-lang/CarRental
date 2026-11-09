namespace CarRental.Core.Pricing;

/// <summary>Єдине місце округлення грошей у проєкті (правила D-12 і D-13).</summary>
public static class Money
{
    /// <summary>Кількість знаків після коми: гривні обліковують до копійок.</summary>
    public const int KopeckDigits = 2;

    /// <summary>Округлює суму до копійок за касовим правилом: 0,5 копійки — угору.</summary>
    /// <param name="amount">Сума, грн.</param>
    /// <returns>Сума, округлена до двох знаків.</returns>
    public static decimal RoundToKopecks(decimal amount) =>
        Math.Round(amount, KopeckDigits, MidpointRounding.AwayFromZero);
}
