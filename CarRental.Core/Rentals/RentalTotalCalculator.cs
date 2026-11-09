using CarRental.Core.Domain;
using CarRental.Core.Pricing;

namespace CarRental.Core.Rentals;

/// <summary>Розрахунок підсумку прокату, розбитий на підпрограми.</summary>
public static class RentalTotalCalculator
{
    /// <summary>Рахує суму рядків прокату з урахуванням знижки за тривалу оренду.</summary>
    /// <param name="lines">Рядки прокату.</param>
    /// <returns>Сума рядків до застосування інших знижок.</returns>
    public static decimal SubtotalOf(IReadOnlyList<RentalLine> lines)
    {
        RentalGuards.EnsureLinesValid(lines);
        decimal subtotal = 0m;
        foreach (RentalLine line in lines)
        {
            decimal sum = line.Days * line.DailyRate;
            if (line.Days >= PricingRules.LongRentalDays)
            {
                sum -= sum * PricingRules.LongRentalRate;
            }

            subtotal += sum;
        }

        return subtotal;
    }

    /// <summary>Рахує знижку постійного клієнта.</summary>
    /// <param name="amount">Поточна сума, грн.</param>
    /// <param name="isRegularCustomer">Ознака постійного клієнта.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal ClientDiscountOf(decimal amount, bool isRegularCustomer)
    {
        RentalGuards.EnsureAmountValid(amount);
        return isRegularCustomer
            ? amount * PricingRules.RegularCustomerRate
            : 0m;
    }

    /// <summary>Рахує знижку за обсягом прокату.</summary>
    /// <param name="amount">Поточна сума, грн.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        RentalGuards.EnsureAmountValid(amount);
        if (amount > PricingRules.LargeRentalFrom)
        {
            return amount * PricingRules.LargeRentalRate;
        }

        if (amount > PricingRules.MediumRentalFrom)
        {
            return amount * PricingRules.MediumRentalRate;
        }

        return 0m;
    }

    /// <summary>Рахує знижку за кодом купона.</summary>
    /// <param name="amount">Поточна сума, грн.</param>
    /// <param name="couponCode">Код купона або null, якщо купона немає.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal CouponDiscountOf(decimal amount, string? couponCode)
    {
        RentalGuards.EnsureAmountValid(amount);
        return couponCode switch
        {
            PricingRules.SaleCoupon
                => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon when amount > PricingRules.CouponFixedFrom
                => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    /// <summary>Рахує знижку за прокат, оформлений у неділю.</summary>
    /// <param name="amount">Поточна сума, грн.</param>
    /// <param name="createdAt">Дата створення прокату.</param>
    /// <returns>Розмір знижки, грн.</returns>
    public static decimal WeekendDiscountOf(decimal amount, DateOnly createdAt)
    {
        RentalGuards.EnsureAmountValid(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday
            ? amount * PricingRules.SundayRate
            : 0m;
    }

    /// <summary>Рахує плату за подачу автомобіля.</summary>
    /// <param name="amount">Поточна сума, грн.</param>
    /// <param name="deliveryPrice">Вартість подачі, грн.</param>
    /// <returns>Плата за подачу, грн; нуль для великих сум.</returns>
    public static decimal DeliveryFeeOf(decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom
            ? deliveryPrice
            : 0m;
    }

    /// <summary>Визначає наступний стан прокату без зміни самого прокату.</summary>
    /// <param name="current">Поточний стан.</param>
    /// <param name="total">Підсумок прокату, грн.</param>
    /// <returns>Стан після розрахунку.</returns>
    public static RentalStatus NextStatusOf(RentalStatus current, decimal total)
        => current switch
        {
            RentalStatus.New when total > 0m => RentalStatus.Confirmed,
            RentalStatus.Confirmed => RentalStatus.Active,
            _ => current,
        };

    /// <summary>Рахує підсумок прокату й наступний стан; не змінює нічого поза собою.</summary>
    /// <param name="request">Запит на розрахунок.</param>
    /// <returns>Підсумок прокату й стан після розрахунку.</returns>
    public static TotalResult Calculate(TotalRequest request)
    {
        RentalGuards.EnsureRequestValid(request);
        decimal total = SubtotalOf(request.Lines);
        total -= ClientDiscountOf(total, request.Customer.IsRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, request.CouponCode);
        total -= WeekendDiscountOf(total, request.CreatedAt);
        total += DeliveryFeeOf(total, request.DeliveryPrice);
        total = request.Status == RentalStatus.Cancelled
            ? 0m
            : Money.RoundToKopecks(Math.Max(total, 0m));
        return new TotalResult(total, NextStatusOf(request.Status, total));
    }
}
