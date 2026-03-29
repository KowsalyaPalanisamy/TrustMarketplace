using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureMarketplace.Models;
using System.ComponentModel.DataAnnotations;

namespace SecureMarketplace.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Find user by email
        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            ErrorMessage = "Invalid login attempt.";
            return Page();
        }

        // Attempt login
        var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            // Redirect based on role
            var roles = await _userManager.GetRolesAsync(user);
            
            if (roles.Contains("Admin"))
                return RedirectToPage("/Admin/Dashboard");
            else if (roles.Contains("Seller"))
                return RedirectToPage("/Products/MyProducts");
            else
                return RedirectToPage("/Products/Index"); // Buyer
        }

        ErrorMessage = "Invalid login attempt.";
        return Page();
    }
}