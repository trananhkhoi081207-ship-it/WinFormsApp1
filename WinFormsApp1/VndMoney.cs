using System;
using System.Globalization;
using System.Text.RegularExpressions;
namespace LabMaintenanceApp
{
    public static class VndMoney
    {
        // SQLite REAL / double chỉ bảo đảm chính xác số nguyên tới giới hạn này.
        public const double MaxExactValue = 9007199254740991d;
        public static bool TryParse(string text, out double value)
        {
            value = 0;
            text = (text ?? "").Trim();
            if (!Regex.IsMatch(text, @"^(?:[0-9]+|[1-9][0-9]{0,2}(?:\.[0-9]{3})+|[1-9][0-9]{0,2}(?:,[0-9]{3})+)$")) return false;
            string digits = text.Replace(".", "").Replace(",", "");
            if (!decimal.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out decimal amount)
                || amount > 9007199254740991m) return false;
            value = (double)amount;
            return true;
        }
        public static string Format(double value) => value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
    }
}
