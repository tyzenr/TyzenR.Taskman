using System.Globalization;

namespace TyzenR.Investor.Managers.Extensions
{
    public static class Extensions
    {        
        public static string FormatIndianCurrency(this double value)
        {
            CultureInfo hindi = new CultureInfo("hi-IN");
            string text = string.Format(hindi, "{0:c}", value);

            return text;
        }

        public static string FormatIndianDate(this DateTime date)
        {
            return date.ToString("dd-MMM-yyyy");
        }

        public static bool IsDayEqual(this DateTime date, DateTime compareDate)
        {
            return (date.Day == compareDate.Day && date.Month == compareDate.Month && date.Year == compareDate.Year);
        }
    }
}
