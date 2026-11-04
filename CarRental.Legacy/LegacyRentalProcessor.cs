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

        decimal subtotal = Subtotal(lines);
        decimal discount = _discounts.For(subtotal, req.Renter);
        decimal shipping = _shipping.For(subtotal - discount);
        decimal total = subtotal - discount + shipping;

        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Renter.Email);
        }

        return _report.Build(req, discount, shipping, total);
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

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
