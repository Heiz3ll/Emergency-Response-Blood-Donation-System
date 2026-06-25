using System.Collections.Concurrent;
using BloodDonorSystem.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace BloodDonorSystem.Infrastructure.Messaging;

public sealed class CommandBus : ICommandBus
{
    private readonly IServiceProvider _provider;

    public CommandBus(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task ExecuteAsync(
        ICommand command,
        CancellationToken cancellationToken = default)
    {
        Type commandType = command.GetType();

        Type handlerType =
            typeof(ICommandHandler<>).MakeGenericType(commandType);

        object handler = _provider.GetRequiredService(handlerType);

        Func<Task> handlerDelegate =
            () => ((dynamic)handler).HandleAsync((dynamic)command, cancellationToken);

        return ExecutePipeline(command, handlerDelegate, cancellationToken);
    }

    public Task<TResult> ExecuteAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default)
    {
        Type commandType = command.GetType();

        Type handlerType =
            typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));

        object handler = _provider.GetRequiredService(handlerType);

        Func<Task<TResult>> handlerDelegate =
            () => ((dynamic)handler).HandleAsync((dynamic)command, cancellationToken);

        return ExecutePipeline(command, handlerDelegate, cancellationToken);
    }

    private async Task ExecutePipeline(
        ICommand command,
        Func<Task> handler,
        CancellationToken cancellationToken)
    {
        Type commandType = command.GetType();

        Type behaviorType =
            typeof(ICommandBehavior<>).MakeGenericType(commandType);

        IEnumerable<object> behaviors =
            _provider.GetServices(behaviorType).OfType<object>();

        Func<Task> pipeline = handler;

        foreach (object behavior in behaviors.Reverse())
        {
            Func<Task> next = pipeline;

            pipeline = () =>
                ((dynamic)behavior).ExecuteAsync(
                    (dynamic)command,
                    next,
                    cancellationToken);
        }

        await pipeline();
    }

    private async Task<TResult> ExecutePipeline<TResult>(
        ICommand command,
        Func<Task<TResult>> handler,
        CancellationToken cancellationToken)
    {
        Type commandType = command.GetType();

        Type behaviorType =
            typeof(ICommandBehavior<,>).MakeGenericType(commandType, typeof(TResult));

        IEnumerable<object> behaviors =
            _provider.GetServices(behaviorType).OfType<object>();

        Func<Task<TResult>> pipeline = handler;

        foreach (object behavior in behaviors.Reverse())
        {
            Func<Task<TResult>> next = pipeline;

            pipeline = () =>
                ((dynamic)behavior).ExecuteAsync(
                    (dynamic)command,
                    next,
                    cancellationToken);
        }

        return await pipeline();
    }
}