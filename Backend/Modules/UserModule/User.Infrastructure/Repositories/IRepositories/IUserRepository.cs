
using FluentResults;
using User.Core.Entity;

namespace User.Infrastructure.Repositories.IRepositories;

public interface IUserRepository
{
    Task<Result> RegisterAsync(
        UserEntity user,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateAsync(
    UserEntity user,
    CancellationToken cancellationToken = default);

    Task<Result<UserEntity>> GetByIdAsync(
       Guid userId,
       CancellationToken cancellationToken = default);

    Task<Result<UserEntity>> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result> LoginAsync(
        UserEntity user,
        string password,
        CancellationToken cancellationToken = default);
}
