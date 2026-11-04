using System;
using System.Collections.Generic;

namespace CarRental.Legacy;

public class Renter
{
    public int Id;
    public string Name = "";
    public string Email = "";
    public string Kind = "regular";
    public int DoneCount;
    public DateTime SinceUtc;
}

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
        string renterKind, int renterDone,
        List<CarLine>? lines, string state,
        string currency, DateTime createdAt, bool sendMail)
    {
        if (lines != null)
        {
            if (lines.Count > 0)
            {
                if (state == "new" || state == "paid")
                {
                    if (renterMail != null &&
                        renterMail.Contains("@"))
                    {
                        _log.Add("ok " + reservationId);
                    }
                    else
                    {
                        return "ERR: mail";
                    }
                }
                else
                {
                    return "ERR: state";
                }
            }
            else
            {
                return "ERR: empty";
            }
        }
        else
        {
            return "ERR: null";
        }

        _tmpSum = 0m;
        for (int i = 0; i < lines.Count; i++)
        {
            _tmpSum += lines[i].Days * lines[i].DailyRate;
        }

        _tmpDiscount = 0m;
        if (renterKind == "vip")
        {
            _tmpDiscount = _tmpSum * 0.15m;
            if (_tmpDiscount > 500m) _tmpDiscount = 500m;
        }
        else if (renterKind == "staff")
        {
            _tmpDiscount = _tmpSum * 0.30m;
            if (_tmpDiscount > 500m) _tmpDiscount = 500m;
        }
        else if (renterDone > 10)
        {
            _tmpDiscount = _tmpSum * 0.05m;
            if (_tmpDiscount > 500m) _tmpDiscount = 500m;
        }

        decimal ship = 0m;
        if (_tmpSum - _tmpDiscount < 1000m) ship = 60m;
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

    public decimal Preview(string renterKind, int renterDone,
        List<CarLine> lines)
    {
        decimal s = 0m;
        foreach (var it in lines)
        {
            s += it.Days * it.DailyRate;
        }
        decimal d = 0m;
        if (renterKind == "vip")
        {
            d = s * 0.15m;
            if (d > 500m) d = 500m;
        }
        else if (renterKind == "staff")
        {
            d = s * 0.30m;
            if (d > 500m) d = 500m;
        }
        else if (renterDone > 10)
        {
            d = s * 0.05m;
            if (d > 500m) d = 500m;
        }
        decimal sh = 0m;
        if (s - d < 1000m) sh = 60m;
        return s - d + sh;
    }

    public string DescribeClient(Renter c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == "vip") s += " [VIP]";
        if (c.DoneCount > 10) s += " [ЛОЯЛЬНИЙ]";
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
