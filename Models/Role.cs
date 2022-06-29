using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace navision.api.Models
{
    public class Role: IdentityRole<int>
    {
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}