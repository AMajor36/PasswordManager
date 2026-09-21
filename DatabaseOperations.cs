using System.Text;
public class PasswordManagement
{
    private readonly AppDbContext database;
    private readonly VaultLockService vaultLockService;
    public PasswordManagement(AppDbContext database, VaultLockService vaultLockService)
    {
        this.database = database;
        this.vaultLockService = vaultLockService;
    }

    public void CreatePassword(string serviceName, string username, string password)
    {
        byte[] vaultKey = vaultLockService.GetVaultKey();
        byte[] encryptedServiceName = Cryptography.Encrypt(Encoding.UTF8.GetBytes(serviceName), vaultKey, out byte[] serviceNameIv);
        byte[] encryptedUsername = Cryptography.Encrypt(Encoding.UTF8.GetBytes(username), vaultKey, out byte[] usernameIv);
        byte[] encryptedPassword = Cryptography.Encrypt(Encoding.UTF8.GetBytes(password), vaultKey, out byte[] passwordIv);

        database.VaultStorage.Add(new VaultStorage
        {
            EncryptedServiceName = encryptedServiceName,
            ServiceNameIv = serviceNameIv,
            EncryptedUsername = encryptedUsername,
            UsernameIv = usernameIv,
            EncryptedPassword = encryptedPassword,
            PasswordIv = passwordIv,
        }
        );
        database.SaveChanges();
    }

    public void DeletePassword(int passwordId)
    {
        var passwordEntry = database.VaultStorage.Find(passwordId);

        if (passwordEntry is not null)
        {
            database.VaultStorage.Remove(passwordEntry);
            database.SaveChanges();
        }
        
    }

    public void UpdatePassword(int passwordId, string newPassword)
    {
        byte[] vaultKey = vaultLockService.GetVaultKey();
        var passwordEntry = database.VaultStorage.Find(passwordId) ?? throw new InvalidOperationException("Password not found.");

        byte[] encryptedPassword = Cryptography.Encrypt(Encoding.UTF8.GetBytes(newPassword), vaultKey, out byte[] passwordIv);

        passwordEntry.EncryptedPassword = encryptedPassword;
        passwordEntry.PasswordIv = passwordIv;
        database.SaveChanges();
        }
    
    public (string username, string password) GetPassword(int passwordId)
    {
        var passwordEntry = database.VaultStorage.Find(passwordId) ?? throw new InvalidOperationException("Password not found.");
        byte[] vaultKey = vaultLockService.GetVaultKey();

        byte[] decryptedUsernameBytes = Cryptography.Decrypt(passwordEntry.EncryptedUsername, vaultKey, passwordEntry.UsernameIv);
        byte[] decryptedPasswordBytes = Cryptography.Decrypt(passwordEntry.EncryptedPassword, vaultKey, passwordEntry.PasswordIv);

        string username = Encoding.UTF8.GetString(decryptedUsernameBytes);
        string password = Encoding.UTF8.GetString(decryptedPasswordBytes);

        return (username, password);
    }

    public void ListPasswords()
    {
        
    }
}