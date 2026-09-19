using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<VaultMetadata> VaultMetadata { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Data/vault.db");
    }
}

public class VaultMetadata
{
    public int Id { get; set; }
    public required byte[] Salt { get; set; }
    public required byte[] PasswordVerificationHash { get; set; }
}