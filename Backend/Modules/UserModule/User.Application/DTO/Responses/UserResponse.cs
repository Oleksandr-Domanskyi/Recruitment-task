using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Application.DTO.Responses
{
    public sealed record UserResponse(
        Guid Id,
        string UserName,
        string Email,
        string? AvatarUrl,
        DateTime CreatedAt);
}