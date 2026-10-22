namespace CarRental.Core.Abstractions;

/// <summary>Сповіщувач: надсилає повідомлення, не знаючи, як саме воно доставляється.</summary>
public interface INotifier
{
    /// <summary>Надсилає повідомлення.</summary>
    /// <param name="message">Текст повідомлення.</param>
    void Notify(string message);
}
