namespace CarRental.Core.Domain;

/// <summary>
/// Орендар — запис довідника учасників. Незмінний: дані клієнта
/// безпечно передавати будь-куди, їх ніхто не перепише.
/// </summary>
public sealed record Renter
{
    /// <summary>Створює орендаря й перевіряє його інваріант.</summary>
    /// <param name="id">Ідентифікатор, більший за нуль.</param>
    /// <param name="fullName">Прізвище та ім'я, не порожні.</param>
    /// <param name="email">Електронна пошта, містить символ @.</param>
    public Renter(int id, string fullName, string email)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!email.Contains('@'))
        {
            throw new ArgumentException("Пошта має містити символ @.", nameof(email));
        }

        Id = id;
        FullName = fullName;
        Email = email;
    }

    /// <summary>Ідентифікатор орендаря.</summary>
    public int Id { get; }

    /// <summary>Прізвище та ім'я орендаря.</summary>
    public string FullName { get; }

    /// <summary>Електронна пошта орендаря.</summary>
    public string Email { get; }

    /// <summary>Ознака постійного клієнта; необов'язкова, задається лише при створенні.</summary>
    public bool IsRegular { get; init; }
}
