using Microsoft.AspNetCore.Http;
using User.Application.CQRS.Command.UpdateUserAvatar;

namespace User.API.Forms;

public sealed record UpdateUserAvatarForm(
    IFormFile File)
{
    public UpdateUserAvatarCommand ToCommand(Guid userId)
        => new(
            userId,
            File.OpenReadStream(),
            File.FileName);
}