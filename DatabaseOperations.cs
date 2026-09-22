using System.Security.Cryptography;
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

    // Creates a new password and saves encryped service name, username and passwored and their IV's to db
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

    // Deletes selected password and saves to db
    public void DeletePassword(int passwordId)
    {
        var passwordEntry = database.VaultStorage.Find(passwordId);

        if (passwordEntry is not null)
        {
            database.VaultStorage.Remove(passwordEntry);
            database.SaveChanges();
        }
        
    }

    // Updates a selected password and saves the new encrpyted password and IV to db
    public void UpdatePassword(int passwordId, string newPassword)
    {
        byte[] vaultKey = vaultLockService.GetVaultKey();
        var passwordEntry = database.VaultStorage.Find(passwordId) ?? throw new InvalidOperationException("Password not found.");

        byte[] encryptedPassword = Cryptography.Encrypt(Encoding.UTF8.GetBytes(newPassword), vaultKey, out byte[] passwordIv);

        passwordEntry.EncryptedPassword = encryptedPassword;
        passwordEntry.PasswordIv = passwordIv;
        database.SaveChanges();
        }
    
    // Decrypts the selected row and returns them
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

    // Decrypts service names and exposes row ID's and returns them for UI handling
    public List<(int Id, string ServiceName)> ListServiceNames()
    {
        var entries = database.VaultStorage.ToList();
        var result = new List<(int Id, string ServiceName)>();

        byte[] vaultKey = vaultLockService.GetVaultKey();

        foreach (var entry in entries)
        {
            byte[] decryptedServiceNameBytes = Cryptography.Decrypt(entry.EncryptedServiceName, vaultKey, entry.ServiceNameIv);

            string serviceName = Encoding.UTF8.GetString(decryptedServiceNameBytes);
            result.Add((entry.Id, serviceName));
        }
        
        return result;
        
    }

    // Generates a random string with customer length between 4 and 100 characters including lowercase and uppercase letters, numbers and symbols and returns it
    public string PasswordGenerator(int passwordLength)
    {
        if (passwordLength < 4 || passwordLength > 100)
        {
            throw new InvalidOperationException("Password length must be between 4 and 100 characters.");
        }
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*-<>?";

        var password = new StringBuilder(passwordLength);

        for (int index = 0; index < passwordLength; index++)
        {
            int position = RandomNumberGenerator.GetInt32(characters.Length);
            password.Append(characters[position]);
        }

        return password.ToString();
    }


}