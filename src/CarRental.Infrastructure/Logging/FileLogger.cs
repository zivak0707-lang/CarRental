using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Logging;

/// <summary>Журнал однієї категорії; пише через спільний FileLoggerProvider.</summary>
internal sealed class FileLogger : ILogger
{
    private readonly FileLoggerProvider _provider;
    private readonly string _category;

    /// <summary>Створює журнал категорії.</summary>
    /// <param name="provider">Спільний запис у файл.</param>
    /// <param name="category">Повна назва категорії; у файл іде лише остання частина.</param>
    public FileLogger(FileLoggerProvider provider, string category)
    {
        _provider = provider;
        _category = category[(category.LastIndexOf('.') + 1)..];
    }

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    /// <inheritdoc/>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = formatter(state, exception);
        string details = exception is null
            ? string.Empty
            : $" | {exception.GetType().Name}: {exception.Message}";
        _provider.Write(logLevel, _category, message + details);
    }
}
