using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureMarketplace.Data;
using SecureMarketplace.Models;
using System.ComponentModel.DataAnnotations;

namespace SecureMarketplace.Pages.Products;

[Authorize(Roles = "Seller")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public ProductInputModel Product { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public class ProductInputModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 999999.99)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            ErrorMessage = "User not found.";
            return Page();
        }

        // SECURE: Using Entity Framework - automatically parameterized
        var product = new Product
        {
            Title = Product.Title,
            Description = Product.Description,
            Price = Product.Price,
            ImageUrl = Product.ImageUrl,
            SellerId = currentUser.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        SuccessMessage = "Product created successfully!";
        ModelState.Clear();
        Product = new ProductInputModel();

        return Page();
    }
}