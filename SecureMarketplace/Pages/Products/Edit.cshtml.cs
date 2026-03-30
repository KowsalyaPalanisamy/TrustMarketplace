using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureMarketplace.Data;
using SecureMarketplace.Models;
using System.ComponentModel.DataAnnotations;

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
    public EditProductModel Product { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class EditProductModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 999999.99)]
        public decimal Price { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            ErrorMessage = "Product not found.";
            return Page();
        }

        // SECURITY: Check if seller owns this product
        if (product.SellerId != currentUser.Id)
        {
            ErrorMessage = "You can only edit your own products.";
            return Page();
        }

        Product = new EditProductModel
        {
            Id = product.Id,
            Title = product.Title ?? string.Empty,
            Description = product.Description ?? string.Empty,
            Price = product.Price
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == Product.Id);

        if (product == null)
        {
            ErrorMessage = "Product not found.";
            return Page();
        }

        // SECURITY: Verify ownership again
        if (product.SellerId != currentUser?.Id)
        {
            ErrorMessage = "You can only edit your own products.";
            return Page();
        }

        // Update fields
        product.Title = Product.Title;
        product.Description = Product.Description;
        product.Price = Product.Price;

        await _context.SaveChangesAsync();

        return RedirectToPage("/Products/Index");
    }
}