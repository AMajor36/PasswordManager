public class VaultLockService
{
    private readonly AppDbContext database;
    private byte[]? vaultKey;
    private bool vaultIsLocked = true;

    public VaultLockService(AppDbContext database)
    {
        this.database = database;
    }
    public void lockVault()
    {
        if (vaultKey != null)
        {
            Array.Clear(vaultKey, 0, vaultKey.Length);
            vaultKey = null;
        }
        vaultIsLocked = true;
        Console.WriteLine("Vault locked.");
    }

    public void UnlockVault(string masterPassword)
    {
        if (Cryptography.VerifyPassword(masterPassword, 
        database.VaultMetadata.First().Salt, 
        database.VaultMetadata.First().PasswordVerificationHash))
        {
        //decrypt the vault and allow the user to access the password manager
        vaultKey = Cryptography.CreateVaultKey(masterPassword, 
        database.VaultMetadata.First().Salt);

        vaultIsLocked = false;
        Console.WriteLine("Vault unlocked.");
        }

        else
        {
            throw new InvalidOperationException("Incorrect master password.");
        }
    }

    public bool isLocked()
    {
        return vaultIsLocked;
    }

    public byte[] GetVaultKey()
    {
        if (vaultIsLocked || vaultKey is null)
        {
            throw new InvalidOperationException("Vault is locked.");
        }
        
        return vaultKey;
    }

    public void resetDeleteVault(string masterPassword)
    {
       if (Cryptography.VerifyPassword(masterPassword, 
       database.VaultMetadata.First().Salt, 
       database.VaultMetadata.First().PasswordVerificationHash))
        {
            database.VaultStorage.RemoveRange(database.VaultStorage);
            database.VaultMetadata.RemoveRange(database.VaultMetadata);
            
            database.SaveChanges();
            lockVault();
        }
        else
        {
            throw new InvalidOperationException("");
        }

    }
}


public class AppInitialisation
{
    private readonly AppDbContext database;
    private readonly VaultLockService vaultLockService;
    public AppInitialisation(AppDbContext database, VaultLockService vaultLockService)
    {
        this.database = database;
        this.vaultLockService = vaultLockService;
    }
    public bool CheckVault()
    {
        bool vaultExists = database.VaultMetadata.Any();

        return vaultExists;
    }

    public void CreateVault(string newMasterPassword)
    {
        var masterPasswordServices = new MasterPasswordServices(database, vaultLockService);
        masterPasswordServices.InitialiseMasterPassword(newMasterPassword);

        Console.WriteLine("Vault created.");
        vaultLockService.UnlockVault(newMasterPassword);
    }
}

