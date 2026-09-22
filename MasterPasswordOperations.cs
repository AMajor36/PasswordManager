using System.Text;

public class MasterPasswordServices
{
    private readonly AppDbContext database;
    private readonly VaultLockService vaultLockService;

    public MasterPasswordServices(AppDbContext database, VaultLockService vaultLockService)
    {
        this.database = database;
        this.vaultLockService = vaultLockService;
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

    public void ChangeMasterPassword(string oldMasterPassword, string newMasterPassword)
    {
        var metadata = database.VaultMetadata.First();

        if (string.IsNullOrEmpty(oldMasterPassword)){
            throw new InvalidOperationException("Master password cannot be empty");
        }

        if (string.IsNullOrEmpty(newMasterPassword)){
            throw new InvalidOperationException("New master password cannot be empty");
        }

        if (!Cryptography.VerifyPassword(oldMasterPassword, metadata.Salt, metadata.PasswordVerificationHash))
        {
            throw new InvalidOperationException("Incorrect master password.");  
        }

            var oldKey = Cryptography.DeriveKey(oldMasterPassword, metadata.Salt);
            
            var newSalt = Cryptography.GenerateNewSalt();
            var newKey = Cryptography.DeriveKey(newMasterPassword, newSalt);

            foreach(var entry in database.VaultStorage.ToList())
            {
                string serviceName = Encoding.UTF8.GetString(Cryptography.Decrypt(entry.EncryptedServiceName, oldKey, entry.ServiceNameIv));
                string username = Encoding.UTF8.GetString(Cryptography.Decrypt(entry.EncryptedUsername, oldKey, entry.UsernameIv));
                string password = Encoding.UTF8.GetString(Cryptography.Decrypt(entry.EncryptedPassword, oldKey, entry.PasswordIv));

                entry.EncryptedServiceName = Cryptography.Encrypt(Encoding.UTF8.GetBytes(serviceName), newKey, out var newServiceNameIv);
                entry.ServiceNameIv = newServiceNameIv;
                entry.EncryptedUsername = Cryptography.Encrypt(Encoding.UTF8.GetBytes(username), newKey, out var newUsernameIv);
                entry.UsernameIv = newUsernameIv;
                entry.EncryptedPassword = Cryptography.Encrypt(Encoding.UTF8.GetBytes(password), newKey, out var newPasswordIv);
                entry.PasswordIv = newPasswordIv;
            }

        metadata.Salt = newSalt;
        metadata.PasswordVerificationHash = Cryptography.DeriveKey(newMasterPassword, newSalt);

        database.SaveChanges();
        vaultLockService.lockVault();
        
    }
}