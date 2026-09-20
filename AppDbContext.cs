using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<VaultMetadata> VaultMetadata { get; set; }
    public DbSet<VaultStorage> VaultStorage { get; set; }

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
    public required string ServiceName { get; set; }
    public required string Username { get; set; }
    public required byte[] EncryptedPassword { get; set; }
    public required byte[] Iv { get; set; }
}