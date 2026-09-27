using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetroVaultWebApp.Models;

namespace RetroVaultWebApp.Data;

public class RetroVaultWebDbContext : DbContext
{
    public RetroVaultWebDbContext(DbContextOptions<RetroVaultWebDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Theme> Themes => Set<Theme>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).HasMaxLength(100);
            e.Property(u => u.Alias).HasMaxLength(100);
            e.Property(u => u.PasswordHash).HasMaxLength(200);
            e.Property(u => u.PreferredCurrency).HasMaxLength(10).HasDefaultValue("NOK");
        });

        modelBuilder.Entity<Theme>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Name).HasMaxLength(100);
            e.Property(t => t.BodyBg).HasMaxLength(20);
            e.Property(t => t.BodyColor).HasMaxLength(20);
            e.Property(t => t.CardBg).HasMaxLength(20);
            e.Property(t => t.CardBorderColor).HasMaxLength(50);
            e.Property(t => t.PrimaryColor).HasMaxLength(20);
            e.Property(t => t.NavbarBg).HasMaxLength(20);
            e.Property(t => t.NavbarTextColor).HasMaxLength(20);
            e.Property(t => t.FooterBg).HasMaxLength(20);
            e.Property(t => t.MutedColor).HasMaxLength(20);
        });

        modelBuilder.Entity<Theme>().HasData(
            new Theme { Id = 1, Name = "Light", IsBuiltIn = true },
            new Theme { Id = 2, Name = "Dark", IsBuiltIn = true }
        );
    }

    public void EnsureSeeded()
    {
        if (!Users.Any())
        {
            var hasher = new PasswordHasher<User>();
            var admin = new User
            {
                Username = "admin",
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow
            };
            admin.PasswordHash = hasher.HashPassword(admin, "pass");
            Users.Add(admin);
            SaveChanges();
        }

        if (!Themes.Any())
        {
            Themes.AddRange(
                new Theme { Id = 1, Name = "Light", IsBuiltIn = true },
                new Theme { Id = 2, Name = "Dark", IsBuiltIn = true }
            );
            SaveChanges();
        }
    }
}
