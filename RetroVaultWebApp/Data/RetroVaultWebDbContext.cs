using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetroVaultWebApp.Models;

namespace RetroVaultWebApp.Data;

public class RetroVaultWebDbContext : DbContext
{
    public RetroVaultWebDbContext(DbContextOptions<RetroVaultWebDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Theme> Themes => Set<Theme>();
    public DbSet<ListItem> ListItems => Set<ListItem>();

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

        modelBuilder.Entity<ListItem>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.ListType).HasMaxLength(20);
            e.Property(l => l.Name).HasMaxLength(200);
            e.HasIndex(l => new { l.ListType, l.Name }).IsUnique();
        });

        modelBuilder.Entity<Theme>().HasData(
            new Theme { Id = 1, Name = "Light", IsBuiltIn = true },
            new Theme { Id = 2, Name = "Dark", IsBuiltIn = true }
        );

        SeedListItems(modelBuilder);
    }

    private static void SeedListItems(ModelBuilder modelBuilder)
    {
        var categories = new[] { "Hardware", "Peripherals", "Games", "Software", "LP", "Books", "Magazines", "Movies", "Other" };
        var systems = new[]
        {
            "PC", "PC Engine/TurboGrafx", "Commodore 64/128", "Commodore Amiga", "Commodore 16/Plus4",
            "Atari 2600", "Atari 800XL", "Atari 7800", "Atari ST",
            "Xbox", "Xbox 360", "Xbox One",
            "Game & Watch", "MSX",
            "Playstation 1", "Playstation 2", "Playstation 3", "Playstation 4", "Playstation Portable (PSP)",
            "Sega Master System", "Sega Mega Drive", "Sega Genesis",
            "NES", "Famicom", "SNES", "Super Famicom", "Nintendo 64",
            "GameBoy", "GameBoy Color", "GameBoy Advance", "Nintendo DS", "Nintendo Switch", "GameCube",
            "Tandy/Radio Shack", "Evercade", "WII", "Other"
        };

        var items = new List<ListItem>();
        int id = 1;

        for (int i = 0; i < categories.Length; i++)
            items.Add(new ListItem { Id = id++, ListType = "Category", Name = categories[i], SortOrder = i });

        for (int i = 0; i < systems.Length; i++)
            items.Add(new ListItem { Id = id++, ListType = "System", Name = systems[i], SortOrder = i });

        modelBuilder.Entity<ListItem>().HasData(items);
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
