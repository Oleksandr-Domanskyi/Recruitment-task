
using FluentResults;
using User.Core.Entity;
using User.Application.Interfaces;
using User.Infrastructure.Repositories.IRepositories;

namespace User.Infrastructure.Services;

public class UserRepositoryServices : IUserRepositoryService
{
    private readonly IUserRepository _repository;

    public UserRepositoryServices(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> RegisterAsync(
        UserEntity user,
        string password,
        CancellationToken cancellationToken = default)
        => await _repository.RegisterAsync(
            user, password, cancellationToken);

    public async Task<Result<UserEntity>> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(
            userId, cancellationToken);

    public async Task<Result<UserEntity>> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
        => await _repository.GetByEmailAsync(
            email, cancellationToken);

    public async Task<Result> LoginAsync(
        UserEntity user,
        string password,
        CancellationToken cancellationToken = default)
        => await _repository.LoginAsync(
            user, password, cancellationToken);

}
