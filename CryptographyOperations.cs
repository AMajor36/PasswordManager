using System.Text;
using System.Security.Cryptography;

public class Cryptography
{
    //variables for salt size, iterations, and key size
    private const int saltSize = 16;
    private const int iterations = 100000;
    private const int keySize = 32;
    private const int ivSize = 12;
    private const int tagSize = 16;

    // Method to derive the key from the master password and salt
    public static byte[] DeriveKey(string masterPassword, Byte[] salt)
    {
        byte[] derivedKey = Rfc2898DeriveBytes.Pbkdf2(
        Encoding.UTF8.GetBytes(masterPassword), 
        salt, 
        iterations, 
        HashAlgorithmName.SHA256, 
        keySize
        );

        return derivedKey;
    }
    
    // Method to generate a new salt
    public static byte[] GenerateNewSalt()
    {
        return RandomNumberGenerator.GetBytes(saltSize);
    }

    // Method to encrypt the password using the derived key
    public static byte[] EncryptPassword(string clearPayload, byte[] derivedKey)
    {
        return unclearPayload;
    };
    
    // Method to decrypt the password using the derived key
    public static string DecryptPassword(byte[] unclearPayload, byte[] derivedKey)
    {
        return clearPayload;
    }

    // Method to verify the password by comparing the derived key with the stored hash
        public static bool VerifyPassword(string MasterPassword, byte [] salt, byte[] hash)
    {
        byte[] candidateKey = Cryptography.DeriveKey(MasterPassword, salt);

        return CryptographicOperations.FixedTimeEquals(candidateKey, hash);
    }
}