using DataMiddleware.Models;

namespace DataMiddleware.Patterns.ChainOfResponsibility;

public abstract class LogHandler
{
    private LogHandler? _nextHandler;

    public LogHandler SetNext(LogHandler next)
    {
        _nextHandler = next;
        return next;
    }

    public void Handle(LogData logData)
    {
        Process(logData);
        _nextHandler?.Handle(logData);
    }

    protected abstract void Process(LogData logData);
}
