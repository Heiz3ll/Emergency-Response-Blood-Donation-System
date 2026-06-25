using BloodDonorSystem.Application.Interfaces;
using BloodDonorSystem.Application.Messaging;

namespace BloodDonorSystem.Infrastructure.Messaging;

public sealed class TransactionBehavior<TCommand>
    : ICommandBehavior<TCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        TCommand command,
        Func<Task> next,
        CancellationToken cancellationToken = default)
    {
        await next();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}