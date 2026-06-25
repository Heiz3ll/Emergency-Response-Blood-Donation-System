namespace BloodDonorSystem.Application.Messaging;

public interface ICommandBus
{
    Task ExecuteAsync(
        ICommand command,
        CancellationToken cancellationToken = default);

    Task<TResult> ExecuteAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default);
}