using System.Globalization;

namespace CarRental.ConsoleApp;

/// <summary>Читання вводу з повтором: некоректний формат — не виняток, а повторний запит.</summary>
internal sealed class ConsoleIo
{
    private const string MomentFormat = "yyyy-MM-dd HH:mm";
    private const string DateFormat = "yyyy-MM-dd";

    private static readonly CultureInfo _invariant = CultureInfo.InvariantCulture;

    private readonly TextReader _input;
    private readonly TextWriter _output;

    /// <summary>Створює ввід-вивід поверх потоків.</summary>
    /// <param name="input">Джерело вводу.</param>
    /// <param name="output">Приймач виводу.</param>
    public ConsoleIo(TextReader input, TextWriter output)
    {
        _input = input;
        _output = output;
    }

    /// <summary>Друкує рядок.</summary>
    /// <param name="text">Текст.</param>
    public void Line(string text) => _output.WriteLine(text);

    /// <summary>Ставить запитання і повертає відповідь без пробілів по краях.</summary>
    /// <param name="prompt">Запитання.</param>
    /// <returns>Відповідь або null, якщо ввід закінчився.</returns>
    public string? Ask(string prompt)
    {
        _output.Write($"{prompt}: ");
        return _input.ReadLine()?.Trim();
    }

    /// <summary>Питає ціле число, доки не введуть коректне.</summary>
    /// <param name="prompt">Запитання.</param>
    /// <returns>Число або null, якщо ввід закінчився.</returns>
    public int? AskInt(string prompt)
    {
        while (Ask(prompt) is string answer)
        {
            if (int.TryParse(answer, CultureInfo.InvariantCulture, out int value))
            {
                return value;
            }

            Line("Потрібне ціле число, наприклад 1001.");
        }

        return null;
    }

    /// <summary>Питає момент у форматі рррр-мм-дд гг:хх; порожній ввід означає «зараз».</summary>
    /// <param name="prompt">Запитання.</param>
    /// <param name="now">Поточний момент.</param>
    /// <returns>Момент або null, якщо ввід закінчився.</returns>
    public DateTimeOffset? AskMoment(string prompt, DateTimeOffset now)
    {
        while (Ask($"{prompt} ({MomentFormat}, Enter — зараз)") is string answer)
        {
            if (answer.Length == 0)
            {
                return now;
            }

            if (DateTime.TryParseExact(
                answer, MomentFormat, _invariant, DateTimeStyles.None, out DateTime at))
            {
                return new DateTimeOffset(at, now.Offset);
            }

            Line($"Потрібен формат {MomentFormat}, наприклад 2026-11-20 10:00.");
        }

        return null;
    }

    /// <summary>Питає дату у форматі рррр-мм-дд; порожній ввід означає «сьогодні».</summary>
    /// <param name="prompt">Запитання.</param>
    /// <param name="today">Сьогоднішня дата.</param>
    /// <returns>Дата або null, якщо ввід закінчився.</returns>
    public DateOnly? AskDate(string prompt, DateOnly today)
    {
        while (Ask($"{prompt} ({DateFormat}, Enter — сьогодні)") is string answer)
        {
            if (answer.Length == 0)
            {
                return today;
            }

            if (DateOnly.TryParseExact(
                answer, DateFormat, _invariant, DateTimeStyles.None, out DateOnly d))
            {
                return d;
            }

            Line($"Потрібен формат {DateFormat}, наприклад 2026-11-20.");
        }

        return null;
    }
}
