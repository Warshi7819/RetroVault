using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RetroVaultWebApp.Data;
using RetroVaultWebApp.Dtos;
using RetroVaultWebApp.Models;

namespace RetroVaultWebApp.Pages.Settings;

public class ThemesModel : PageModel
{
    private readonly RetroVaultWebDbContext _db;

    public ThemesModel(RetroVaultWebDbContext db)
    {
        _db = db;
    }

    public List<ThemeDto> Themes { get; set; } = [];
    public int? ActiveThemeId { get; set; }
    public ThemeDto? EditTheme { get; set; }
    public bool IsEditing => Request.Query.ContainsKey("edit");
    public bool IsCopying => Request.Query.ContainsKey("copy");

    public async Task OnGetAsync()
    {
        ActiveThemeId = GetActiveThemeId();
        await LoadThemes();

        if (IsEditing)
        {
            var sourceId = int.Parse(Request.Query["edit"]!);
            EditTheme = await _db.Themes.Where(t => t.Id == sourceId)
                .Select(t => ToDto(t)).FirstOrDefaultAsync();
        }
        else if (IsCopying)
        {
            var sourceId = int.Parse(Request.Query["copy"]!);
            EditTheme = await _db.Themes.Where(t => t.Id == sourceId)
                .Select(t => ToDto(t)).FirstOrDefaultAsync();
        }
    }

    public async Task<IActionResult> OnPostSelectAsync(int id)
    {
        var theme = await _db.Themes.FindAsync(id);
        if (theme is null) return RedirectToPage();

        SetThemeCookie(ToDto(theme));
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCreateAsync(ThemeDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
        var theme = new Theme
        {
            UserId = userId,
            Name = dto.Name,
            BodyBg = dto.BodyBg,
            BodyColor = dto.BodyColor,
            CardBg = dto.CardBg,
            CardBorderColor = dto.CardBorderColor,
            PrimaryColor = dto.PrimaryColor,
            NavbarBg = dto.NavbarBg,
            NavbarTextColor = dto.NavbarTextColor,
            FooterBg = dto.FooterBg,
            MutedColor = dto.MutedColor
        };
        _db.Themes.Add(theme);
        await _db.SaveChangesAsync();

        SetThemeCookie(ToDto(theme));
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateAsync(int id, ThemeDto dto)
    {
        var existing = await _db.Themes.FirstOrDefaultAsync(t => t.Id == id && !t.IsBuiltIn);
        if (existing is null) return RedirectToPage();

        existing.Name = dto.Name;
        existing.BodyBg = dto.BodyBg;
        existing.BodyColor = dto.BodyColor;
        existing.CardBg = dto.CardBg;
        existing.CardBorderColor = dto.CardBorderColor;
        existing.PrimaryColor = dto.PrimaryColor;
        existing.NavbarBg = dto.NavbarBg;
        existing.NavbarTextColor = dto.NavbarTextColor;
        existing.FooterBg = dto.FooterBg;
        existing.MutedColor = dto.MutedColor;
        await _db.SaveChangesAsync();

        if (GetActiveThemeId() == id)
        {
            SetThemeCookie(ToDto(existing));
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var existing = await _db.Themes.FirstOrDefaultAsync(t => t.Id == id && !t.IsBuiltIn);
        if (existing is null) return RedirectToPage();

        _db.Themes.Remove(existing);
        await _db.SaveChangesAsync();

        if (GetActiveThemeId() == id)
        {
            Response.Cookies.Delete("RetroVaultTheme");
        }

        return RedirectToPage();
    }

    private int? GetActiveThemeId()
    {
        var cookie = Request.Cookies["RetroVaultTheme"];
        if (string.IsNullOrEmpty(cookie)) return null;

        if (cookie == "light") return 1;
        if (cookie == "dark") return 2;
        if (cookie.StartsWith('{'))
        {
            try { return JsonSerializer.Deserialize<JsonElement>(cookie).GetProperty("Id").GetInt32(); }
            catch { return null; }
        }
        return null;
    }

    private void SetThemeCookie(ThemeDto theme)
    {
        string cookieValue;
        if (theme.IsBuiltIn)
        {
            cookieValue = theme.Name.ToLowerInvariant();
        }
        else
        {
            cookieValue = JsonSerializer.Serialize(new
            {
                theme.Id,
                theme.BodyBg, theme.BodyColor, theme.CardBg, theme.CardBorderColor,
                theme.PrimaryColor, theme.NavbarBg, theme.NavbarTextColor,
                theme.FooterBg, theme.MutedColor
            });
        }

        Response.Cookies.Append("RetroVaultTheme", cookieValue, new CookieOptions
        {
            MaxAge = TimeSpan.FromDays(365),
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });
    }

    private async Task LoadThemes()
    {
        Themes = await _db.Themes
            .OrderBy(t => t.Id)
            .Select(t => ToDto(t))
            .ToListAsync();
    }

    private static ThemeDto ToDto(Theme t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        IsBuiltIn = t.IsBuiltIn,
        BodyBg = t.BodyBg,
        BodyColor = t.BodyColor,
        CardBg = t.CardBg,
        CardBorderColor = t.CardBorderColor,
        PrimaryColor = t.PrimaryColor,
        NavbarBg = t.NavbarBg,
        NavbarTextColor = t.NavbarTextColor,
        FooterBg = t.FooterBg,
        MutedColor = t.MutedColor
    };
}
