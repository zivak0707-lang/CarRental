namespace CarRental.Core.Reporting;

/// <summary>Виведення готового звіту в потік: команда, що нічого не повертає.</summary>
public sealed class RentalReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>Створює принтер, що пише у вказаний потік.</summary>
    /// <param name="output">Потік виведення.</param>
    public RentalReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Записує звіт у потік виведення.</summary>
    /// <param name="report">Текст звіту, не порожній.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
