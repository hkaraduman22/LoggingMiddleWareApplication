using DataMiddleware.Models;

namespace DataMiddleware.Patterns.ChainOfResponsibility;

public class SecurityHandler : LogHandler
{
    protected override void Process(LogData logData)
    {
        logData.SensitiveData.TcKimlik = MaskNumeric(logData.SensitiveData.TcKimlik, 4);
        logData.SensitiveData.CreditCard = MaskNumeric(logData.SensitiveData.CreditCard, 4);
        logData.SensitiveData.Email = MaskEmail(logData.SensitiveData.Email);
    }

    private static string MaskNumeric(string value, int keepLast)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= keepLast)
        {
            return value;
        }

        return new string('*', value.Length - keepLast) + value[^keepLast..];
    }

    private static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return email;
        }

        var parts = email.Split('@');
        if (parts.Length != 2)
        {
            return email;
        }

        var local = parts[0];
        var domain = parts[1];
        var visible = local.Length <= 2 ? local : local[..2];
        var masked = visible + new string('*', Math.Max(0, local.Length - visible.Length));
        return masked + "@" + domain;
    }
}
