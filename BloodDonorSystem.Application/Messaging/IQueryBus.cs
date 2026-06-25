namespace BloodDonorSystem.Application.Messaging;

public interface IQueryBus
{
    Task<TResult> SendAsync<TResult>(
        IQuery<TResult> query,
        CancellationToken cancellationToken = default);
}