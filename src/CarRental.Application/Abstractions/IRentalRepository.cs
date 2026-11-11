using CarRental.Domain.Rentals;

namespace CarRental.Application.Abstractions;

/// <summary>Сховище документів оренди; реалізація — в інфраструктурі.</summary>
public interface IRentalRepository
{
    /// <summary>Усі оренди в порядку номерів.</summary>
    /// <returns>Незмінний перелік оренд.</returns>
    IReadOnlyList<Rental> All();

    /// <summary>Шукає оренду за номером договору.</summary>
    /// <param name="number">Номер договору.</param>
    /// <returns>Оренда або null, якщо її немає.</returns>
    Rental? Find(int number);

    /// <summary>Наступний вільний номер договору.</summary>
    /// <returns>Номер, більший за всі наявні.</returns>
    int NextNumber();

    /// <summary>Додає нову оренду (зберігається викликом Save).</summary>
    /// <param name="rental">Нова оренда.</param>
    void Add(Rental rental);

    /// <summary>Зберігає всі зміни.</summary>
    void Save();
}
