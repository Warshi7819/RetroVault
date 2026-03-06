using RetroVault.Shared;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Services;
using System.Security.Cryptography;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add cookie authentication
builder.Services.AddAuthentication("MyCookieAuth") 
    .AddCookie("MyCookieAuth", options => 
    { 
        options.LoginPath = "/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Always requires HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// Configure the antiforgery options directly to please penetration test...
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});


// Add rate limiting services - to be used to protect the login endpoint
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Define a "LoginPolicy" that allows 3 requests every 1 minute per IP
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

builder.Services.AddHttpClient<VaultApiClient>(client =>
{
    client.BaseAddress = new Uri($"{vaultOptions.BaseServerUrl}api/");
});

builder.Services.AddHttpClient<ThumbnailService>(client =>
{
    client.BaseAddress = new Uri(vaultOptions.BaseServerUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var isDev = app.Environment.IsDevelopment();

    // Generate a per-request nonce
    var nonceBytes = new byte[16];
    RandomNumberGenerator.Fill(nonceBytes);
    var nonce = Convert.ToBase64String(nonceBytes);

    // Store nonce so Razor pages can use it
    context.Items["CSP-Nonce"] = nonce;
    var scriptSrc = $"script-src 'self' 'nonce-{nonce}' https://challenges.cloudflare.com https://static.cloudflareinsights.com";
    var connectSrc = "connect-src 'self' https://*.cloudflare.com";

    if (isDev)
    {
        // Use Dev settings instead of prod settings.
        connectSrc += " http://localhost:* https://localhost:* ws://localhost:* wss://localhost:*";
        scriptSrc = $"script-src 'self' 'unsafe-inline' https://challenges.cloudflare.com https://static.cloudflareinsights.com";
    }
    var csp =
        "default-src 'self'; " +
        scriptSrc + "; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' https://fonts.gstatic.com; " +
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
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.Run();
