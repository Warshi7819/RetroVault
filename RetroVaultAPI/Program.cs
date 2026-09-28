using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RetroVaultAPI.Data;
using RetroVaultAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<RetroVaultContext>(options => 
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Media library files (Library/{id}/{Audio,Documents,Images,Software,Videos})
builder.Services.AddSingleton(new LibraryFileService());

// Large uploads (media library holds big PDFs, images and software)
var maxUploadBytes = 4L * 1024 * 1024 * 1024;
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = maxUploadBytes);
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxUploadBytes;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Check if Thumbnail dir exists, if not create
var thumbDir = Path.Combine(Environment.CurrentDirectory, "Thumbnails");
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
