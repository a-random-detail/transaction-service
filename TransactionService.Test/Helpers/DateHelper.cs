using System.Globalization;

namespace TransactionService.Test.Helpers;

public class DateHelper
{
    public static DateOnly ParseStringToDateOnly(string date)
    {
        return DateOnly.Parse(date);
    }

    public static DateTime ParseStringToDateTime(string requestTransactionDate)
    {
        return DateTime.ParseExact(requestTransactionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
    }
}