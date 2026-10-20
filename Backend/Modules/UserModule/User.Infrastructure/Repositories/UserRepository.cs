
using FluentResults;
using Microsoft.AspNetCore.Identity;
using User.Core.Entity;
using User.Core.Entity.Enums;
using User.Infrastructure.Repositories.IRepositories;

namespace User.Infrastructure.Repositories;

public sealed class UserRepository(
    UserManager<UserEntity> userManager,
    SignInManager<UserEntity> signInManager)
    : IUserRepository
{
    public async Task<Result> RegisterAsync(
        UserEntity user,
        string password,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return Result.Fail(
                result.Errors.Select(x => x.Description));
        }

        var roleResult = await userManager.AddToRoleAsync(user, Roles.User.ToString());

        if (!roleResult.Succeeded)
        {
            return Result.Fail(
                roleResult.Errors.Select(x => x.Description));
        }

        return Result.Ok();
    }
    public async Task<Result<UserEntity>> GetByIdAsync(
       Guid userId,
       CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Fail<UserEntity>("User not found.");

        return Result.Ok(user);
    }

    public async Task<Result<UserEntity>> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Fail<UserEntity>("User not found.");

        return Result.Ok(user);
    }

    public async Task<Result> LoginAsync(
        UserEntity user,
        string password,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Result.Fail("Account is temporarily locked.");

        if (result.IsNotAllowed)
            return Result.Fail("Sign-in is not allowed.");

        if (!result.Succeeded)
            return Result.Fail("Invalid credentials.");

        return Result.Ok();
    }
}
