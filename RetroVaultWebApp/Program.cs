using RetroVault.Shared;
using RetroVaultWebApp.Config;
using RetroVaultWebApp.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add cookie authentication
builder.Services.AddAuthentication("MyCookieAuth") 
    .AddCookie("MyCookieAuth", options => 
    { 
        options.LoginPath = "/Login"; 
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
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
