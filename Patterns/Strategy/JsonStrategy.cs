using System.Text.Json;
using DataMiddleware.Models;

namespace DataMiddleware.Patterns.Strategy;

public class JsonStrategy : IOutputStrategy
{
    public string Format(LogData data)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        return JsonSerializer.Serialize(data, options);
    }
}
