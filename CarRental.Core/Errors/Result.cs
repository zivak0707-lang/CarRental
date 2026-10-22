namespace CarRental.Core.Errors;

/// <summary>Фабрика результатів: Result.Ok(значення) або Result.Fail&lt;T&gt;(пояснення).</summary>
public static class Result
{
    /// <summary>Успішний результат із готовим значенням.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="value">Значення.</param>
    /// <returns>Результат з IsSuccess = true.</returns>
    public static Result<T> Ok<T>(T value) => new(true, value, string.Empty);

    /// <summary>Невдалий результат із поясненням для користувача.</summary>
    /// <typeparam name="T">Тип значення, яке не вдалося отримати.</typeparam>
    /// <param name="error">Пояснення, не порожнє.</param>
    /// <returns>Результат з IsSuccess = false.</returns>
    public static Result<T> Fail<T>(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        return new(false, default, error);
    }
}
