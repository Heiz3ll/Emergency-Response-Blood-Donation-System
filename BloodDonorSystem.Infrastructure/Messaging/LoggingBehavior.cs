using BloodDonorSystem.Application.Messaging;
using Microsoft.Extensions.Logging;

namespace BloodDonorSystem.Infrastructure.Messaging;

public sealed class LoggingBehavior<TCommand>
    : ICommandBehavior<TCommand>
    where TCommand : ICommand
{
    private readonly ILogger<LoggingBehavior<TCommand>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TCommand>> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(
        TCommand command,
        Func<Task> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing {Command}", typeof(TCommand).Name);

        await next();

        _logger.LogInformation("Executed {Command}", typeof(TCommand).Name);
    }
}