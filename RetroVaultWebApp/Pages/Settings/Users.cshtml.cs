using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroVaultWebApp.Data;
using RetroVaultWebApp.Dtos;
using RetroVaultWebApp.Models;

namespace RetroVaultWebApp.Pages.Settings;

[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly RetroVaultWebDbContext _db;

    public UsersModel(RetroVaultWebDbContext db)
    {
        _db = db;
    }

    public List<UserDto> Users { get; set; } = [];
    public string ErrorMessage { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        await LoadUsers();
    }

    public async Task<IActionResult> OnPostCreateAsync(string newUsername, string? newAlias, string newPassword, bool newIsAdmin)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
        {
            ErrorMessage = "Username is required";
            return RedirectToPage();
        }
        if (string.IsNullOrEmpty(newPassword))
        {
            ErrorMessage = "Password is required";
            return RedirectToPage();
        }
        if (await _db.Users.AnyAsync(u => u.Username.ToLower() == newUsername.Trim().ToLower()))
        {
            ErrorMessage = "A user with that name already exists";
            return RedirectToPage();
        }

        var user = new User
        {
            Username = newUsername.Trim(),
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, newPassword),
            Alias = newAlias,
            IsAdmin = newIsAdmin,
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleAdminAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        if (user.IsAdmin && !await HasAnotherEnabledAdmin(id))
        {
            ErrorMessage = "Cannot demote the last enabled admin account";
            return RedirectToPage();
        }

        user.IsAdmin = !user.IsAdmin;
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleDisableAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        if (!user.IsDisabled && user.IsAdmin && !await HasAnotherEnabledAdmin(id))
        {
            ErrorMessage = "Cannot disable the last enabled admin account";
            return RedirectToPage();
        }

        user.IsDisabled = !user.IsDisabled;
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSetAliasAsync(int id, string? alias)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        user.Alias = alias;
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        if (user.IsAdmin && !await HasAnotherEnabledAdmin(id))
        {
            ErrorMessage = "Cannot delete the last enabled admin account";
            return RedirectToPage();
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public bool IsLastEnabledAdmin(UserDto user)
    {
        if (!user.IsAdmin || user.IsDisabled) return false;
        return Users.Count(u => u.IsAdmin && !u.IsDisabled && u.Id != user.Id) == 0;
    }

    private async Task<bool> HasAnotherEnabledAdmin(int excludeId)
    {
        return await _db.Users.AnyAsync(u => u.Id != excludeId && u.IsAdmin && !u.IsDisabled);
    }

    private async Task LoadUsers()
    {
        Users = await _db.Users
            .OrderBy(u => u.Username)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Alias = u.Alias,
                IsAdmin = u.IsAdmin,
                IsDisabled = u.IsDisabled,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
    }
}
