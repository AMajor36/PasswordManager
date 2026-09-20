public class VaultLockService
{
    private readonly AppDbContext database;
    private byte[]? vaultKey;

    private bool vaultIsLocked = true;

    public VaultLockService(AppDbContext database){
        this.database = database;
    }
    public void lockVault()
    {
        if (vaultKey != null){
            Array.Clear(vaultKey, 0, vaultKey.Length);
            vaultKey = null;
        }
        vaultIsLocked = true;
    }

    public void unlockVault()
    {
            while (true)
            {
            string masterPassword = ReadPassword("Enter master password:  ");

            if (Cryptography.VerifyPassword(masterPassword, database.VaultMetadata.First().Salt, database.VaultMetadata.First().PasswordVerificationHash))
            {
                Console.WriteLine("Master password verified successfully.");
                //decrypt the vault and allow the user to access the password manager
                vaultKey = Cryptography.CreateVaultKey(masterPassword, database.VaultMetadata.First().Salt);
                vaultIsLocked = false;
                break;
            }
            else
            {
                Console.WriteLine("Incorrect master password.");
            }
        }
    }
    public static string ReadPassword(string prompt)
    {
        Console.Write(prompt);
        string password = Console.ReadLine() ?? "";
        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Password cannot be empty. Please try again.");
            return ReadPassword(prompt);
        }

        return password;
    }

    public bool isLocked()
    {
        return vaultIsLocked;
    }

    public byte[] GetVaultKey(){
        if (vaultIsLocked || vaultKey is null){
            throw new InvalidOperationException("Vault is locked.");
        }
        
        return vaultKey;
        }
    }


public class AppInitialisation{
    private readonly AppDbContext database;
    public AppInitialisation(AppDbContext database){
        this.database = database;
    }
    public void InitialiseOrUnlockVault()
    {
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
