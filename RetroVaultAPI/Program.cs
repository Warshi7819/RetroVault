using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RetroVaultAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<RetroVaultContext>(options => 
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Check if Thumbnail dir exists, if not create
var thumbDir = Path.Combine(AppContext.BaseDirectory, "Thumbnails");
if (!System.IO.Directory.Exists(thumbDir))
{ 
    Directory.CreateDirectory(thumbDir);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(thumbDir),
    RequestPath = "/thumbnails"
});


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
