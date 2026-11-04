namespace CarRental.Legacy;

public sealed record RentalRequest(
    int ReservationId,
    Renter Renter,
    IReadOnlyList<CarLine>? Lines,
    DocumentState State,
    string Currency,
    bool SendMail);
