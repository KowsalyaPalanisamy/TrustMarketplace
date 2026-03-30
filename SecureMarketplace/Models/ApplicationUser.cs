using Microsoft.AspNetCore.Identity;

namespace SecureMarketplace.Models;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string? FullName { get; set; }
    
    [PersonalData]
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    
    // Add this collection if you want to access products from user
    public virtual ICollection<Product>? Products { get; set; }
}