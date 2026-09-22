public class Program
{
    public static void Main()
    {
        using var database = new AppDbContext();
        var vaultLockService = new VaultLockService(database);
        var appInitialisation = new AppInitialisation(database, vaultLockService);
        var passwordManagement = new PasswordManagement(database, vaultLockService);
        
        database.Database.EnsureCreated();
        passwordManagement.ListServiceNames();
    }
}