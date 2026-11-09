using System.Text;

namespace CarRental.Core.Reporting;

/// <summary>Формує текст простого звіту з готових рядків.</summary>
public sealed class ReportBuilder
{
    /// <summary>Заголовок звіту.</summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>Будує текст: заголовок великими літерами, далі рядки звіту.</summary>
    /// <param name="rows">Рядки звіту.</param>
    /// <returns>Текст звіту.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var text = new StringBuilder();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (var row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }
}
