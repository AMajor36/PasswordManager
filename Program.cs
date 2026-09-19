//start
//checks if vault is created, if not it will create a new vault and ask the user to create a master password
//when the user creates a master password, it will generate and store a new salt and derive a key from the master password and salt
//if vault is created, it will ask the user to enter the master password and verify it against the stored hash and salt

public class Program
{
    public static void Main(string[] args)
    {
        using var database = new AppDbContext();

        database.Database.EnsureCreated();

        bool vaultExists = database.VaultMetadata.Any();

        if (!vaultExists)
        {
            string masterPassword = VaultLockService.ReadPassword("Create a new master password: ");
            var masterPasswordServices = new MasterPasswordServices(database);
            masterPasswordServices.InitialiseMasterPassword(masterPassword);
            Console.WriteLine("Vault created.");
        }
        else
        {
            var vaultLockService = new VaultLockService(database);
            vaultLockService.unlockVault();
        }
    }
}