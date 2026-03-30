using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureMarketplace.Data;
using SecureMarketplace.Models;

namespace SecureMarketplace.Pages.Products;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public ProductDetailViewModel? Product { get; set; }

    public async Task OnGetAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product != null)
        {
            Product = new ProductDetailViewModel
            {
                Id = product.Id,
                Title = product.Title ?? "Untitled",
                Description = product.Description ?? "No description",
                Price = product.Price,
                SellerName = product.Seller?.FullName ?? product.Seller?.Email ?? "Unknown",
                CreatedAt = product.CreatedAt
            };
        }
    }
}

public class ProductDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}