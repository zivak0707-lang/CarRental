using System.Text;

namespace CarRental.Core.Reporting;

/// <summary>Формує текст простого звіту з готових рядків.</summary>
public sealed class ReportBuilder
{
    /// <summary>Заголовок звіту.</summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>Навмисне порушення для перевірки конвеєра.</summary>
    /// <param name="count">Кількість позицій.</param>
    /// <returns>Опис кількості.</returns>
    public string Describe(int count)
    {
        string note;
        return Title + ": " + count;
    }

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
