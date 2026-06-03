namespace DataMiddleware.Models;

public class LogData
{
    public string Timestamp { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string TransactionNo { get; set; } = string.Empty;
    public SensitiveData SensitiveData { get; set; } = new();
    public TransactionDetails TransactionDetails { get; set; } = new();
    public string LogLevel { get; set; } = string.Empty;
    public bool IsCritical { get; set; }
    public string SummaryMessage { get; set; } = string.Empty;
}

public class SensitiveData
{
    public string TcKimlik { get; set; } = string.Empty;
    public string CreditCard { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class TransactionDetails
{
    public string Symbol { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
