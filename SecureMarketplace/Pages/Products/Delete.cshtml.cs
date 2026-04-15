using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureMarketplace.Data;
using SecureMarketplace.Models;

namespace SecureMarketplace.Pages.Products;

[Authorize(Roles = "Seller,Admin")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Product? Product { get; set; }
    public string? ErrorMessage { get; set; }

    // ==========================================
    // VULNERABLE CODE - No ownership verification
    // Any seller can delete ANY product
    // ==========================================
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // DANGEROUS: No check if seller owns this product
        Product = await _context.Products
                    .Include(p => p.Seller)
                    .FirstOrDefaultAsync(p => p.Id == id);

        if (Product == null)
        {
            ErrorMessage = "Product not found.";
            return Page();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            // DANGEROUS: No ownership verification before deletion
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Products/Index");
    }
}