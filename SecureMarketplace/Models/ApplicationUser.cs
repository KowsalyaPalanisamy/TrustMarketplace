using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SecureMarketplace.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; } 
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}