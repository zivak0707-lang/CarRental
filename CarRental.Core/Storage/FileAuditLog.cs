namespace CarRental.Core.Storage;

/// <summary>
/// Журнал аудиту у файлі: кожен рядок — час, операція й номер документа.
/// Володіє відкритим потоком, тому реалізує IDisposable.
/// </summary>
public sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly TimeProvider _clock;
    private bool _disposed;

    /// <summary>Відкриває файл аудиту для дописування.</summary>
    /// <param name="path">Шлях до файлу.</param>
    /// <param name="clock">Джерело поточного часу.</param>
    public FileAuditLog(string path, TimeProvider clock)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(clock);
        _clock = clock;
        _writer = new StreamWriter(path, append: true);
    }

    /// <summary>Дописує рядок аудиту.</summary>
    /// <param name="operation">Назва операції, наприклад reservation.paid.</param>
    /// <param name="documentId">Номер документа.</param>
    public void Write(string operation, int documentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        _writer.WriteLine($"{_clock.GetLocalNow():O};{operation};{documentId}");
    }

    /// <summary>Скидає буфер на диск і закриває файл; повторний виклик нічого не робить.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
