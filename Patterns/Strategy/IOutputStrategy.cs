using DataMiddleware.Models;

namespace DataMiddleware.Patterns.Strategy;

public interface IOutputStrategy
{
    string Format(LogData data);
}
