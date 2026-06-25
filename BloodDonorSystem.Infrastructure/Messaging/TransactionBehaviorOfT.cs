using BloodDonorSystem.Application.Interfaces;
using BloodDonorSystem.Application.Messaging;

namespace BloodDonorSystem.Infrastructure.Messaging;

public sealed class TransactionBehavior<TCommand, TResult>
    : ICommandBehavior<TCommand, TResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResult> ExecuteAsync(
        TCommand command,
        Func<Task<TResult>> next,
        CancellationToken cancellationToken = default)
    {
        TResult result = await next();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return result;
    }
}