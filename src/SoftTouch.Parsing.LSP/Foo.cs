using Microsoft.Extensions.Logging;

namespace SoftTouch.Parsing.LSP;


internal class Foo
{
    private readonly ILogger<Foo> _logger;

    public Foo(ILogger<Foo> logger)
    {
        logger.LogInformation("inside ctor");
        _logger = logger;
    }

    public void SayFoo()
    {
        _logger.LogInformation("Fooooo!");
    }
}