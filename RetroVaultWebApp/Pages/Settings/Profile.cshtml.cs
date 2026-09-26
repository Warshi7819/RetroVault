using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroVaultWebApp.Data;
using RetroVaultWebApp.Models;

namespace RetroVaultWebApp.Pages.Settings;

public class ProfileModel : PageModel
{
    private readonly RetroVaultWebDbContext _db;

    public ProfileModel(RetroVaultWebDbContext db)
    {
        _db = db;
    }

    public string ErrorMessage { get; set; } = string.Empty;
    public string? CurrentAlias { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(userId);
        CurrentAlias = user?.Alias;
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateAliasAsync(string? alias)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        user.Alias = alias;
        await _db.SaveChangesAsync();

        var identity = User.Identity as ClaimsIdentity;
        var existingAlias = User.FindFirst("Alias");
        if (existingAlias is not null)
        {
            identity?.RemoveClaim(existingAlias);
        }

        if (!string.IsNullOrEmpty(alias))
        {
            identity?.AddClaim(new Claim("Alias", alias));
        }
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity!));

        TempData["Success"] = "Alias updated";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAsync(string currentPassword, string newPassword, string confirmPassword)
    {
        if (string.IsNullOrEmpty(currentPassword))
        {
            ErrorMessage = "Current password is required";
            return Page();
        }
        if (string.IsNullOrEmpty(newPassword))
        {
            ErrorMessage = "New password is required";
            return Page();
        }
        if (newPassword != confirmPassword)
        {
            ErrorMessage = "New password and confirmation do not match";
            return Page();
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
        if (result == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "Current password is incorrect";
            return Page();
        }

        user.PasswordHash = hasher.HashPassword(user, newPassword);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Password changed";
        return RedirectToPage();
    }
}
