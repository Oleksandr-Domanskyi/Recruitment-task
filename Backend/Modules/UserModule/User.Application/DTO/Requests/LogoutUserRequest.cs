using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Application.DTO.Requests
{
    public sealed record LogoutUserRequest(
        string RefreshToken);
}