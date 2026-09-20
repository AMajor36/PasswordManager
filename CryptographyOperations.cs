using System.Text;
using System.Security.Cryptography;

public class Cryptography
{
    //variables for salt size, iterations, key size, IV size, and tag size
    private const int saltSize = 16;
    private const int iterations = 100000;
    private const int keySize = 32;
    private const int ivSize = 12;
    private const int tagSize = 16;

    // Method to derive the key from the master password and salt
    public static byte[] DeriveKey(string masterPassword, byte[] salt)
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
    
    // Method to verify the password by comparing the derived key with the stored hash
        public static bool VerifyPassword(string MasterPassword, byte [] salt, byte[] hash)
    {
        byte[] candidateKey = DeriveKey(MasterPassword, salt);

        return CryptographicOperations.FixedTimeEquals(candidateKey, hash);
    }

    public static byte[] CreateVaultKey(string masterPassword, byte[] salt)
    {
        return DeriveKey(masterPassword, salt);
    }

    public static byte[] Encrypt(byte[] plaintext, byte[] key, out byte[] iV)
    {
        iV = RandomNumberGenerator.GetBytes(ivSize);
        byte [] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[tagSize];

        using var aes = new AesGcm(key, tagSize);
        aes.Encrypt(iV, plaintext, ciphertext, tag);

        byte[] encryptedText = new byte[ciphertext.Length + tag.Length];
        Buffer.BlockCopy(ciphertext, 0, encryptedText, 0, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, encryptedText, ciphertext.Length, tag.Length);

        return encryptedText;

    }
}