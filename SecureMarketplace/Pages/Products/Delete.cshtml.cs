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

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToPage("/Account/Login");
        }

        Product = await _context.Products
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (Product == null)
        {
            ErrorMessage = "Product not found.";
            return Page();
        }

        // Check permission: Admin can delete any, Sellers only their own
        var isAdmin = User.IsInRole("Admin");
        var isOwner = Product.SellerId == currentUser.Id;

        if (!isAdmin && !isOwner)
        {
            ErrorMessage = "You don't have permission to delete this product.";
            return Page();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return RedirectToPage("/Products/Index");
        }

        var isAdmin = User.IsInRole("Admin");
        var isOwner = product.SellerId == currentUser?.Id;

        if (isAdmin || isOwner)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Products/Index");
    }
}