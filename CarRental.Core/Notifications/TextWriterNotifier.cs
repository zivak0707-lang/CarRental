using CarRental.Core.Abstractions;

namespace CarRental.Core.Notifications;

/// <summary>Сповіщувач, що пише повідомлення в переданий потік.</summary>
public sealed class TextWriterNotifier : INotifier
{
    private readonly TextWriter _output;

    /// <summary>Створює сповіщувач для вказаного потоку.</summary>
    /// <param name="output">Потік виведення (консоль, файл або TextWriter.Null у тестах).</param>
    public TextWriterNotifier(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <inheritdoc/>
    public void Notify(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        _output.WriteLine($"Повідомлення: {message}");
    }
}
