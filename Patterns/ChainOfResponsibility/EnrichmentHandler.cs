using DataMiddleware.Models;

namespace DataMiddleware.Patterns.ChainOfResponsibility;

public class EnrichmentHandler : LogHandler
{
    protected override void Process(LogData logData)
    {
        logData.IsCritical = logData.LogLevel.Equals("CRITICAL", StringComparison.OrdinalIgnoreCase)
            || logData.LogLevel.Equals("WARNING", StringComparison.OrdinalIgnoreCase);

        logData.SummaryMessage = $"Transaction {logData.TransactionNo} for {logData.TransactionDetails.Symbol} " +
                                 $"{logData.TransactionDetails.Action} {logData.TransactionDetails.Amount:C2} " +
                                 $"was received with log level {logData.LogLevel}.";
    }
}
