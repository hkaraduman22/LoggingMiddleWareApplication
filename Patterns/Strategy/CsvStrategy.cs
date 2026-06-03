using DataMiddleware.Models;

namespace DataMiddleware.Patterns.Strategy;

public class CsvStrategy : IOutputStrategy
{
    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return '"' + value.Replace("\"", "\"\"") + '"';
        }

        return value;
    }

    public string Format(LogData data)
    {
        var header = "Timestamp,SenderId,TransactionNo,Symbol,Action,Amount,LogLevel,IsCritical,SummaryMessage,TcKimlik,CreditCard,Email";
        var line = string.Join(",",
            Escape(data.Timestamp),
            Escape(data.SenderId),
            Escape(data.TransactionNo),
            Escape(data.TransactionDetails.Symbol),
            Escape(data.TransactionDetails.Action),
            data.TransactionDetails.Amount.ToString("F2"),
            Escape(data.LogLevel),
            data.IsCritical.ToString(),
            Escape(data.SummaryMessage),
            Escape(data.SensitiveData.TcKimlik),
            Escape(data.SensitiveData.CreditCard),
            Escape(data.SensitiveData.Email));

        return header + "\n" + line;
    }
}
