using BloodDonorSystem.Domain.Entities;
using BloodDonorSystem.Domain.ValueObjects;

namespace BloodDonorSystem.Domain.Repository;

public interface IUserRepository
{
    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default);

    //Task DeleteAsync(
    //    User user,
    //    CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Email email,
        CancellationToken cancellationToken = default);
}