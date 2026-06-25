using BloodDonorSystem.Domain.Entities;
using BloodDonorSystem.Domain.Repository;
using BloodDonorSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BloodDonorSystem.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(
            user,
            cancellationToken);
    }

    public Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);

        return Task.CompletedTask;
    }

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email.Value == email.Value,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(
                x => x.Email.Value == email.Value,
                cancellationToken);
    }
}