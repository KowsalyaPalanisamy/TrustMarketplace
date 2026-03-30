using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureMarketplace.Data;
using SecureMarketplace.Models;

namespace SecureMarketplace.Pages.Products;

[Authorize] // All logged-in users can view products
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<ProductViewModel> Products { get; set; } = new();
    public string? CurrentUserId { get; set; }

    public async Task OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        CurrentUserId = currentUser?.Id;

        // SECURE: Using Entity Framework (parameterized queries automatically)
        var products = await _context.Products
            .Include(p => p.Seller)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        Products = products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Title = p.Title ?? "Untitled",
            Description = p.Description ?? "No description",
            Price = p.Price,
            SellerId = p.SellerId,
            SellerName = p.Seller?.FullName ?? p.Seller?.Email ?? "Unknown Seller",
            CreatedAt = p.CreatedAt
        }).ToList();
    }
}

public class ProductViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}