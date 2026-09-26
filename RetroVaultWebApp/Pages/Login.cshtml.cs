using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using RetroVaultWebApp.Data;
using RetroVaultWebApp.Models;

namespace RetroVaultWebApp.Pages;

[EnableRateLimiting("LoginPolicy")]
public class LoginModel : PageModel
{
    private readonly RetroVaultWebDbContext _db;

    public LoginModel(RetroVaultWebDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public string? Username { get; set; }

    [BindProperty]
    public string? Password { get; set; }

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Invalid username or password";
            return Page();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == Username);
        if (user is null)
        {
            ErrorMessage = "Invalid username or password";
            return Page();
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "Invalid username or password";
            return Page();
        }

        if (user.IsDisabled)
        {
            ErrorMessage = "This account has been disabled";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };
        if (!string.IsNullOrEmpty(user.Alias))
        {
            claims.Add(new Claim("Alias", user.Alias));
        }
        if (user.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToPage("/Index");
    }
}
