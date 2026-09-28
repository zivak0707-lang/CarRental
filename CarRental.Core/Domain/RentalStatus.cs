namespace CarRental.Core.Domain;

/// <summary>Стан прокату в його життєвому циклі.</summary>
public enum RentalStatus
{
    New = 0,
    Confirmed = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4,
}