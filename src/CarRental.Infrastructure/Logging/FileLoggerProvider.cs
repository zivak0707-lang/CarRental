using System.Globalization;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Logging;

/// <summary>
/// Журнал у текстовий файл: рядок «час | рівень | джерело | повідомлення».
/// Персональних даних і вмісту файлів у повідомленнях немає за домовленістю (docs/spec.md).
/// </summary>
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _path;
    private readonly TimeProvider _clock;
    private readonly object _gate = new();

    /// <summary>Створює журнал; теку файлу буде створено за потреби.</summary>
    /// <param name="path">Шлях до файлу журналу.</param>
    /// <param name="clock">Годинник для позначок часу.</param>
    public FileLoggerProvider(string path, TimeProvider clock)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(clock);
        _path = path;
        _clock = clock;
        string? directory = Path.GetDirectoryName(Path.GetFullPath(path));
        Directory.CreateDirectory(directory!);
    }

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName)
    {
        ArgumentNullException.ThrowIfNull(categoryName);
        return new FileLogger(this, categoryName);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Файл відкривається на кожен запис, тож звільняти нічого
    }

    /// <summary>Дописує рядок журналу; виклики з різних потоків не перемішуються.</summary>
    /// <param name="level">Рівень запису.</param>
    /// <param name="category">Джерело (коротка назва класу).</param>
    /// <param name="message">Текст запису.</param>
    internal void Write(LogLevel level, string category, string message)
    {
        string time = _clock.GetLocalNow()
            .ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        string line = $"{time} | {level} | {category} | {message}{Environment.NewLine}";
        lock (_gate)
        {
            File.AppendAllText(_path, line);
        }
    }
}
