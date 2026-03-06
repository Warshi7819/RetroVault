using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using RetroVault.Shared;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Services;
using System.Security.Claims;

[EnableRateLimiting("LoginPolicy")]
public class LoginModel : PageModel
{
    private readonly VaultOptions _options;

    public LoginModel(IOptions<VaultOptions> options)
    {
        _options = options.Value;
    }


    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPost()
    {
        // Retrieve valid credentials from configuration
        string validUser = _options.User["Username"];
        string validPass = _options.User["Password"];

        var passwordProcessed = Password;
        if (_options.User["UseSHA256Hash"] != "false")
        {
            // Hash supplied password with SHA256 when configured to do so
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(Password);
                var hash = sha256.ComputeHash(bytes);
                passwordProcessed = Convert.ToBase64String(hash);
            }
        }


        if (Username == validUser && passwordProcessed == validPass)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Username)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return RedirectToPage("/Index");
        }

        ErrorMessage = "Invalid username or password";
        return Page();
    }
}