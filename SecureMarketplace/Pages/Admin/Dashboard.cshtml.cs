using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureMarketplace.Data;
using SecureMarketplace.Models;

namespace SecureMarketplace.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int TotalProducts { get; set; }
    public int TotalUsers { get; set; }
    public int TotalSellers { get; set; }
    public int TotalBuyers { get; set; }
    public List<UserInfo> RecentUsers { get; set; } = new();
    public List<ProductInfo> AllProducts { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Statistics
        TotalProducts = await _context.Products.CountAsync();
        
        var allUsers = await _userManager.Users.ToListAsync();
        TotalUsers = allUsers.Count;
        
        // Count users by role
        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Seller")) TotalSellers++;
            if (roles.Contains("Buyer")) TotalBuyers++;
        }

        // Recent users (last 5)
        RecentUsers = allUsers
            .OrderByDescending(u => u.RegisteredAt)
            .Take(5)
            .Select(async u => new UserInfo
            {
                Email = u.Email ?? "Unknown",
                FullName = u.FullName ?? "Unknown",
                Role = (await _userManager.GetRolesAsync(u)).FirstOrDefault() ?? "User"
            })
            .Select(t => t.Result)
            .ToList();

        // All products with seller names
        var products = await _context.Products
            .Include(p => p.Seller)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        AllProducts = products.Select(p => new ProductInfo
        {
            Id = p.Id,
            Title = p.Title ?? "Untitled",
            Price = p.Price,
            SellerName = p.Seller?.FullName ?? p.Seller?.Email ?? "Unknown",
            CreatedAt = p.CreatedAt
        }).ToList();
    }
}

public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class ProductInfo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}