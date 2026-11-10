namespace CarRental.Domain.Errors;

/// <summary>
/// Порушено правило предметної області. Rule — ідентифікатор правила,
/// за яким помилку можна знайти в журналі (наприклад, rental.transition).
/// </summary>
public class DomainRuleException : Exception
{
    /// <summary>Створює виняток для порушеного правила.</summary>
    /// <param name="rule">Ідентифікатор правила, не порожній.</param>
    /// <param name="message">Повідомлення українською з конкретними значеннями.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        Rule = rule;
    }

    /// <summary>Ідентифікатор порушеного правила.</summary>
    public string Rule { get; }
}
