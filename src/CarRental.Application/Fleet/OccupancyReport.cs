namespace CarRental.Application.Fleet;

/// <summary>Звіт «Зайнятість автопарку» на дату (UC-06).</summary>
/// <param name="Day">Дата звіту.</param>
/// <param name="Rows">Рядки за класами в порядку переліку CarClass.</param>
/// <param name="Total">Підсумковий рядок.</param>
public sealed record OccupancyReport(
    DateOnly Day, IReadOnlyList<OccupancyRow> Rows, OccupancyRow Total);
