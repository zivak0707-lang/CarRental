using System;
using System.Collections.Generic;

namespace CarRental.Legacy;

public class CarLine
{
    public string Plate = "";
    public int Days;
    public decimal DailyRate;
    public string CarClass = "";
}

public class LegacyRentalProcessor
{
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly List<string> _log = new();

    public string Handle(RentalRequest req)
    {
        string? error = Validate(req);
        if (error != null)
        {
            return error;
        }

        _log.Add("ok " + req.ReservationId);
        IReadOnlyList<CarLine> lines = req.Lines!;

        _tmpSum = Subtotal(lines);
        _tmpDiscount = DiscountOf(_tmpSum, req.Renter);
        decimal ship = ShippingOf(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = "Документ #" + req.ReservationId + "\n";
        txt += "Клієнт: " + req.Renter.Name + "\n";
        for (int i = 0; i < lines.Count; i++)
        {
            txt += lines[i].Plate + " x" + lines[i].Days + " = " +
                (lines[i].Days * lines[i].DailyRate)
                    .ToString("0.00") + " " + req.Currency + "\n";
        }
        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
            " " + req.Currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + req.Currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + req.Currency + "\n";

        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Renter.Email);
        }
        return txt;
    }

    private static decimal Subtotal(IReadOnlyList<CarLine> lines)
    {
        decimal sum = 0m;
        foreach (CarLine line in lines)
        {
            sum += line.Days * line.DailyRate;
        }

        return sum;
    }

    private static decimal DiscountOf(decimal subtotal, Renter renter)
    {
        decimal rate = renter.Kind switch
        {
            RenterKind.Vip => PricingRules.VipRate,
            RenterKind.Staff => PricingRules.StaffRate,
            _ => renter.IsLoyal ? PricingRules.LoyalRate : 0m,
        };
        return Math.Min(subtotal * rate, PricingRules.MaxDiscount);
    }

    private static decimal ShippingOf(decimal payable) =>
        payable < PricingRules.FreeShipFrom ? PricingRules.ShipCost : 0m;

    private static string? Validate(RentalRequest req)
    {
        if (req.Lines is null)
        {
            return "ERR: null";
        }

        if (req.Lines.Count == 0)
        {
            return "ERR: empty";
        }

        if (req.State is not (DocumentState.New or DocumentState.Paid))
        {
            return "ERR: state";
        }

        if (!req.Renter.HasValidEmail)
        {
            return "ERR: mail";
        }

        return null;
    }

    public decimal Preview(Renter renter, IReadOnlyList<CarLine> lines)
    {
        decimal subtotal = Subtotal(lines);
        decimal discount = DiscountOf(subtotal, renter);
        decimal payable = subtotal - discount;
        return payable + ShippingOf(payable);
    }

    public string DescribeClient(Renter c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == RenterKind.Vip) s += " [VIP]";
        if (c.DoneCount > PricingRules.LoyalRentals) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
