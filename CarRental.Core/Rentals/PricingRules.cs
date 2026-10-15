namespace CarRental.Core.Rentals;

/// <summary>Тарифні правила й пороги розрахунку підсумку прокату.</summary>
public static class PricingRules
{
    /// <summary>Кількість діб в одному рядку, від якої діє знижка за тривалу оренду.</summary>
    public const int LongRentalDays = 10;

    /// <summary>Частка знижки за тривалу оренду.</summary>
    public const decimal LongRentalRate = 0.05m;

    /// <summary>Частка знижки постійного клієнта.</summary>
    public const decimal RegularCustomerRate = 0.07m;

    /// <summary>Сума, понад яку прокат вважається великим, грн.</summary>
    public const decimal LargeRentalFrom = 5000m;

    /// <summary>Частка знижки на великий прокат.</summary>
    public const decimal LargeRentalRate = 0.10m;

    /// <summary>Сума, понад яку прокат вважається середнім, грн.</summary>
    public const decimal MediumRentalFrom = 2000m;

    /// <summary>Частка знижки на середній прокат.</summary>
    public const decimal MediumRentalRate = 0.05m;

    /// <summary>Частка знижки за прокат, оформлений у неділю.</summary>
    public const decimal SundayRate = 0.02m;

    /// <summary>Частка знижки за купоном SALE10.</summary>
    public const decimal CouponSaleRate = 0.10m;

    /// <summary>Фіксована знижка за купоном MINUS200, грн.</summary>
    public const decimal CouponFixed = 200m;

    /// <summary>Сума, понад яку діє купон MINUS200, грн.</summary>
    public const decimal CouponFixedFrom = 1000m;

    /// <summary>Сума, від якої подачу автомобіля не оплачують, грн.</summary>
    public const decimal FreeDeliveryFrom = 1000m;

    /// <summary>Код купона зі знижкою у відсотках.</summary>
    public const string SaleCoupon = "SALE10";

    /// <summary>Код купона з фіксованою знижкою.</summary>
    public const string FixedCoupon = "MINUS200";
}
