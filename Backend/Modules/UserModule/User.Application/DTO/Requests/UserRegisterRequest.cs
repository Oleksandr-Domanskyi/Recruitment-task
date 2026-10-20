using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Application.DTO.Requests
{

    public sealed record UserRegisterRequest(
        string UserName,
        string Email,
        string Password);
}