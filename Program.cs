//start
//checks if vault is created, if not it will create a new vault and ask the user to create a master password
//when the user creates a master password, it will generate and store a new salt and derive a key from the master password and salt
//if vault is created, it will ask the user to enter the master password and verify it against the stored hash and salt

public class Program
{
    public static void Main()
    {
        using var database = new AppDbContext();
        var vaultLockService = new VaultLockService(database);
        var appInitialisation = new AppInitialisation(database, vaultLockService);
        var passwordManagement = new PasswordManagement(database, vaultLockService);
        
        database.Database.EnsureCreated();
        appInitialisation.InitialiseOrUnlockVault();
        passwordManagement.ListServiceNames();
    }
}