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
    private readonly DiscountPolicy _discounts = new();
    private readonly ShippingPolicy _shipping = new();
    private readonly ReportBuilder _report = new();
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
        _tmpDiscount = _discounts.For(_tmpSum, req.Renter);
        decimal ship = _shipping.For(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Renter.Email);
        }

        return _report.Build(req, _tmpDiscount, ship, total);
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
        decimal discount = _discounts.For(subtotal, renter);
        decimal payable = subtotal - discount;
        return payable + _shipping.For(payable);
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
