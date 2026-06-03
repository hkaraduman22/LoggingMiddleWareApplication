using DataMiddleware.Models;
using DataMiddleware.Patterns.ChainOfResponsibility;
using DataMiddleware.Patterns.Strategy;
using Microsoft.AspNetCore.Mvc;

namespace DataMiddleware.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] LogData logData)
    {
        var securityHandler = new SecurityHandler();
        var enrichmentHandler = new EnrichmentHandler();
        securityHandler.SetNext(enrichmentHandler);
        securityHandler.Handle(logData);

        var strategies = new IOutputStrategy[]
        {
            new HtmlStrategy(),
            new CsvStrategy(),
            new JsonStrategy()
        };

        foreach (var strategy in strategies)
        {
            var formatted = strategy.Format(logData);
            Console.WriteLine($"--- {strategy.GetType().Name} Output ---");
            Console.WriteLine(formatted);
            Console.WriteLine();
        }

        return Ok();
    }
}
