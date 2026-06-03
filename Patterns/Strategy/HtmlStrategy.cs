using System.Text;
using DataMiddleware.Models;

namespace DataMiddleware.Patterns.Strategy;

public class HtmlStrategy : IOutputStrategy
{
    public string Format(LogData data)
    {
        var builder = new StringBuilder();
        builder.AppendLine("<html>");
        builder.AppendLine("<body>");
        builder.AppendLine($"<h1>Log Record</h1>");
        builder.AppendLine($"<p><strong>Timestamp:</strong> {data.Timestamp}</p>");
        builder.AppendLine($"<p><strong>Sender:</strong> {data.SenderId}</p>");
        builder.AppendLine($"<p><strong>Transaction:</strong> {data.TransactionNo}</p>");
        builder.AppendLine($"<p><strong>Symbol:</strong> {data.TransactionDetails.Symbol}</p>");
        builder.AppendLine($"<p><strong>Action:</strong> {data.TransactionDetails.Action}</p>");
        builder.AppendLine($"<p><strong>Amount:</strong> {data.TransactionDetails.Amount:C2}</p>");
        builder.AppendLine($"<p><strong>Log Level:</strong> {data.LogLevel}</p>");
        builder.AppendLine($"<p><strong>Critical:</strong> {data.IsCritical}</p>");
        builder.AppendLine($"<p><strong>Summary:</strong> {data.SummaryMessage}</p>");
        builder.AppendLine($"<p><strong>TC Kimlik:</strong> {data.SensitiveData.TcKimlik}</p>");
        builder.AppendLine($"<p><strong>Credit Card:</strong> {data.SensitiveData.CreditCard}</p>");
        builder.AppendLine($"<p><strong>Email:</strong> {data.SensitiveData.Email}</p>");
        builder.AppendLine("</body>");
        builder.AppendLine("</html>");
        return builder.ToString();
    }
}
