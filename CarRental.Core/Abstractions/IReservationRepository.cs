using CarRental.Core.Domain;

namespace CarRental.Core.Abstractions;

/// <summary>Сховище бронювань: відповідає лише за зберігання.</summary>
public interface IReservationRepository
{
    /// <summary>Зберігає нове бронювання.</summary>
    /// <param name="reservation">Бронювання.</param>
    void Add(Reservation reservation);

    /// <summary>Шукає бронювання за номером.</summary>
    /// <param name="id">Номер бронювання.</param>
    /// <returns>Знайдене бронювання або null.</returns>
    Reservation? GetById(int id);

    /// <summary>Повертає всі збережені бронювання.</summary>
    /// <returns>Список бронювань лише для читання.</returns>
    IReadOnlyList<Reservation> GetAll();
}
