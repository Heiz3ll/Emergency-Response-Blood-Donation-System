namespace BloodDonorSystem.Application.Messaging;

public interface ICommandBehavior<TCommand>
{
    Task ExecuteAsync(
        TCommand command,
        Func<Task> next,
        CancellationToken cancellationToken = default);
}