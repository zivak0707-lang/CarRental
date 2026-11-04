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

    public string Handle(int reservationId, int renterId,
        string renterName, string? renterMail,
        RenterKind renterKind, int renterDone,
        List<CarLine>? lines, DocumentState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        string? error = Validate(lines, state, renterMail);
        if (error != null)
        {
            return error;
        }

        _log.Add("ok " + reservationId);

        _tmpSum = 0m;
        for (int i = 0; i < lines!.Count; i++)
        {
            _tmpSum += lines[i].Days * lines[i].DailyRate;
        }

        _tmpDiscount = 0m;
        if (renterKind == RenterKind.Vip)
        {
            _tmpDiscount = _tmpSum * PricingRules.VipRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
        else if (renterKind == RenterKind.Staff)
        {
            _tmpDiscount = _tmpSum * PricingRules.StaffRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
        else if (renterDone > PricingRules.LoyalRentals)
        {
            _tmpDiscount = _tmpSum * PricingRules.LoyalRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }

        decimal ship = 0m;
        if (_tmpSum - _tmpDiscount < PricingRules.FreeShipFrom) ship = PricingRules.ShipCost;
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = "Документ #" + reservationId + "\n";
        txt += "Клієнт: " + renterName + "\n";
        for (int i = 0; i < lines.Count; i++)
        {
            txt += lines[i].Plate + " x" + lines[i].Days + " = " +
                (lines[i].Days * lines[i].DailyRate)
                    .ToString("0.00") + " " + currency + "\n";
        }
        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
            " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + currency + "\n";

        if (sendMail)
        {
            _log.Add("mail -> " + renterMail);
        }
        return txt;
    }

    private static string? Validate(List<CarLine>? lines,
        DocumentState state, string? renterMail)
    {
        if (lines is null)
        {
            return "ERR: null";
        }

        if (lines.Count == 0)
        {
            return "ERR: empty";
        }

        if (state is not (DocumentState.New or DocumentState.Paid))
        {
            return "ERR: state";
        }

        if (renterMail is null || !renterMail.Contains("@"))
        {
            return "ERR: mail";
        }

        return null;
    }

    public decimal Preview(RenterKind renterKind, int renterDone,
        List<CarLine> lines)
    {
        decimal s = 0m;
        foreach (var it in lines)
        {
            s += it.Days * it.DailyRate;
        }
        decimal d = 0m;
        if (renterKind == RenterKind.Vip)
        {
            d = s * PricingRules.VipRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        else if (renterKind == RenterKind.Staff)
        {
            d = s * PricingRules.StaffRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        else if (renterDone > PricingRules.LoyalRentals)
        {
            d = s * PricingRules.LoyalRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        decimal sh = 0m;
        if (s - d < PricingRules.FreeShipFrom) sh = PricingRules.ShipCost;
        return s - d + sh;
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
