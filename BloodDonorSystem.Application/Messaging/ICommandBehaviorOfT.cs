namespace BloodDonorSystem.Application.Messaging;

public interface ICommandBehavior<TCommand, TResult>
{
    Task<TResult> ExecuteAsync(
        TCommand command,
        Func<Task<TResult>> next,
        CancellationToken cancellationToken = default);
}