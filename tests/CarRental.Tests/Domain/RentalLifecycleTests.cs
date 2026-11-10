using CarRental.Domain.Errors;
using CarRental.Domain.Rentals;
using CarRental.Tests.Doubles;

namespace CarRental.Tests.Domain;

/// <summary>Таблиця переходів стану: docs/spec.md, розділ 5.</summary>
public class RentalLifecycleTests
{
    [Theory]
    [InlineData(RentalStatus.Reserved, RentalEvent.PickUp, RentalStatus.Active)]
    [InlineData(RentalStatus.Reserved, RentalEvent.Cancel, RentalStatus.Cancelled)]
    [InlineData(RentalStatus.Active, RentalEvent.Close, RentalStatus.Closed)]
    public void Next_AllowedTransition_ReturnsNewStatus(
        RentalStatus from, RentalEvent rentalEvent, RentalStatus expected)
    {
        Assert.Equal(expected, RentalTransitions.Next(from, rentalEvent));
    }

    [Theory]
    [InlineData(RentalStatus.Reserved, RentalEvent.Close)]
    [InlineData(RentalStatus.Active, RentalEvent.PickUp)]
    [InlineData(RentalStatus.Active, RentalEvent.Cancel)]
    [InlineData(RentalStatus.Closed, RentalEvent.PickUp)]
    [InlineData(RentalStatus.Closed, RentalEvent.Close)]
    [InlineData(RentalStatus.Closed, RentalEvent.Cancel)]
    [InlineData(RentalStatus.Cancelled, RentalEvent.PickUp)]
    [InlineData(RentalStatus.Cancelled, RentalEvent.Close)]
    [InlineData(RentalStatus.Cancelled, RentalEvent.Cancel)]
    public void Next_ForbiddenTransition_ThrowsDomainException(
        RentalStatus from, RentalEvent rentalEvent)
    {
        var ex = Assert.Throws<InvalidRentalStateException>(
            () => RentalTransitions.Next(from, rentalEvent));

        Assert.Equal(from, ex.Status);
        Assert.Equal("rental.transition", ex.Rule);
    }

    [Fact]
    public void PickUp_Reserved_BecomesActiveWithTime()
    {
        var rental = Sample.Reserved(1001, "AA1001AA");

        rental.PickUp(Sample.Now);

        Assert.Equal(RentalStatus.Active, rental.Status);
        Assert.Equal(Sample.Now, rental.PickedUpAt);
    }
}
