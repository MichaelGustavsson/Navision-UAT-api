using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace navision.api.Models
{
    public class User: IdentityUser<int>
    {
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}