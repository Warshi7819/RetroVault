using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using RetroVault.Shared;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Data;
using RetroVaultWebApp.Services;
using System.Security.Cryptography;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Login");
});

// Large uploads (media library holds big PDFs, images and software)
var maxUploadBytes = 4L * 1024 * 1024 * 1024;
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = maxUploadBytes);
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxUploadBytes;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

builder.Services.AddDbContext<RetroVaultWebDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WebAppDb")));

builder.Services.AddDataProtection();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("LoginPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.Configure<VaultOptions>(
    builder.Configuration.GetSection("VaultOptions"));

var vaultOptions = builder.Configuration
    .GetSection("VaultOptions")
    .Get<VaultOptions>();

if (vaultOptions == null)
{
    throw new InvalidOperationException("Missing VaultOptions in appsettings.json file");
}

builder.Services.AddHttpClient<VaultApiClient>(client =>
{
    client.BaseAddress = new Uri($"{vaultOptions.BaseServerUrl}api/");
});

builder.Services.AddHttpClient<ThumbnailService>(client =>
{
    client.BaseAddress = new Uri(vaultOptions.BaseServerUrl);
});

builder.Services.AddTransient<LibraryProxyService>();

builder.Services.AddHttpClient(PriceChartingUpdateService.ApiClientName, client =>
{
    client.BaseAddress = new Uri($"{vaultOptions.BaseServerUrl}api/");
});

builder.Services.AddHttpClient("PriceChartingScrape", client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
        "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
});

builder.Services.AddSingleton<PriceChartingUpdateService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<PriceChartingUpdateService>());
builder.Services.AddSingleton<ExchangeRateService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RetroVaultWebDbContext>();
    db.Database.Migrate();
    db.EnsureSeeded();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    var isDev = app.Environment.IsDevelopment();

    var nonceBytes = new byte[16];
    RandomNumberGenerator.Fill(nonceBytes);
    var nonce = Convert.ToBase64String(nonceBytes);

    context.Items["CSP-Nonce"] = nonce;
    var scriptSrc = $"script-src 'self' 'nonce-{nonce}' https://challenges.cloudflare.com https://static.cloudflareinsights.com";
    var connectSrc = "connect-src 'self' https://*.cloudflare.com";

    if (isDev)
    {
        connectSrc += " http://localhost:* https://localhost:* ws://localhost:* wss://localhost:*";
        scriptSrc = $"script-src 'self' 'unsafe-inline' https://challenges.cloudflare.com https://static.cloudflareinsights.com";
    }
    var csp =
        "default-src 'self'; " +
        scriptSrc + "; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' https://cdn.jsdelivr.net https://fonts.gstatic.com; " +
        connectSrc + "; " +
        "frame-src https://challenges.cloudflare.com; " +
        "object-src 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "frame-ancestors 'none'; " +
        "upgrade-insecure-requests;";

    context.Response.Headers["Content-Security-Policy"] = csp;
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Media library downloads, proxied from the API so the API is never exposed publicly
app.MapGet("/Files/Download/{id:int}/{category}/{*path}", async (HttpContext context, int id, string category, string path, LibraryProxyService proxy) =>
{
    await proxy.ProxyFileAsync(context, id, category, path);
}).RequireAuthorization();

app.Run();
