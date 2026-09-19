public class MasterPasswordServices
{
    private readonly AppDbContext database;

    public MasterPasswordServices(AppDbContext database)
    {
        this.database = database;
    }

    public void InitialiseMasterPassword(string masterPassword)
    {
        byte[] salt = Cryptography.GenerateNewSalt();
        byte[] derivedKey = Cryptography.DeriveKey(masterPassword, salt);

        database.VaultMetadata.Add(new VaultMetadata
        {
            Salt = salt,
            PasswordVerificationHash = derivedKey
        }
        );
        database.SaveChanges();
    }


/*
    public void ChangeMasterPassword()
    {
        string oldMasterPassword = Console.WriteLine("Enter master password: ");
        if (Cryptography.VerifyPassword(oldMasterPassword, database.VaultMetadata.First().Salt, database.VaultMetadata.First().PasswordVerificationHash))
        {
            Console.WriteLine("Enter a new password: ")
            InitialiseMasterPassword(newMasterPassword);
        }

    }
*/







}