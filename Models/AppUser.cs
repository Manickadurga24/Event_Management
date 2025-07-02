using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EventManagementUpdatedProject.Models
{
    public class AppUser : IdentityUser<int> //handles authentication and authorization
    {
        public required string Name { get; set; }
        public required string ContactNumber { get; set; }
        public required UserRole Type { get; set; }
    }
}
