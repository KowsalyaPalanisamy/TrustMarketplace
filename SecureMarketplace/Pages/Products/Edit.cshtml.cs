using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureMarketplace.Data;
using SecureMarketplace.Models;

namespace SecureMarketplace.Pages.Products;

[Authorize(Roles = "Seller")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public Product Product { get; set; } = new();

    public string? ErrorMessage { get; set; }

    // ==========================================
    // VULNERABLE CODE - No ownership verification
    // Any seller can edit ANY product by changing id in URL
    // ==========================================
    public async Task<IActionResult> OnGetAsync(int id)
    {
        // DANGEROUS: No check if current seller owns this product
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        
        if (product == null)
        {
            ErrorMessage = "Product not found.";
            return Page();
        }

        Product = product;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingProduct = await _context.Products.FindAsync(Product.Id);
        if (existingProduct == null)
        {
            ErrorMessage = "Product not found.";
            return Page();
        }

        // DANGEROUS: No ownership verification before update
        existingProduct.Title = Product.Title;
        existingProduct.Description = Product.Description;
        existingProduct.Price = Product.Price;

        await _context.SaveChangesAsync();
        return RedirectToPage("/Products/Index");
    }
}