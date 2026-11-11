namespace CarRental.Application.Common;

/// <summary>
/// Результат операції з очікуваною відмовою (некоректний ввід): або значення, або пояснення.
/// Створюється лише через Result.Ok і Result.Fail.
/// </summary>
/// <typeparam name="T">Тип значення.</typeparam>
public sealed class Result<T>
{
    /// <summary>Створює результат; викликається лише фабрикою Result.</summary>
    /// <param name="isSuccess">Ознака успіху.</param>
    /// <param name="value">Значення при успіху.</param>
    /// <param name="error">Пояснення при невдачі.</param>
    internal Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>Чи вдалася операція.</summary>
    public bool IsSuccess { get; }

    /// <summary>Значення; має сенс лише при IsSuccess = true.</summary>
    public T? Value { get; }

    /// <summary>Пояснення помилки для користувача; порожнє при успіху.</summary>
    public string Error { get; }
}
