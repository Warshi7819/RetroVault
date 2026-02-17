using Microsoft.EntityFrameworkCore;
using RetroVault.Shared.Models;

namespace RetroVaultAPI.Data
{
    public class RetroVaultContext: DbContext
    {

        public RetroVaultContext(DbContextOptions<RetroVaultContext> options) : base(options)
        {
        }
        public DbSet<VaultItem> VaultItems { get; set; }
    }
}
