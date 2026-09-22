using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<VaultMetadata> VaultMetadata { get; set; } = null!;
    public DbSet<VaultStorage> VaultStorage { get; set; } = null!;

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

public class VaultStorage
{
    public int Id { get; set; }
    public required byte[] EncryptedServiceName { get; set; }
    public required byte[] ServiceNameIv { get; set; }
    public required byte[] EncryptedUsername { get; set; }
    public required byte[] UsernameIv { get; set; }
    public required byte[] EncryptedPassword { get; set; }
    public required byte[] PasswordIv { get; set; }
}