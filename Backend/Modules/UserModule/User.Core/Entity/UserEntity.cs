using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace User.Core.Entity
{
    public class UserEntity : IdentityUser<Guid>
    {
        public string? AvatarUrl { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public void UpdateAvatar(string? avatarUrl)
        {
            AvatarUrl = avatarUrl;
        }
    }
}