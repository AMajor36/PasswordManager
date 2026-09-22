using System.Text;
using System.Security.Cryptography;

//Stores all cryptography methods
public class Cryptography
{
    //variables for salt size, iterations, key size, IV size, and tag size
    private const int saltSize = 16;
    private const int iterations = 100000;
    private const int keySize = 32;
    private const int ivSize = 12;
    private const int tagSize = 16;

    //Derives key from master password and salt
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
    
    //Generates a new salt and returns it
    public static byte[] GenerateNewSalt()
    {
        return RandomNumberGenerator.GetBytes(saltSize);
    }
    
    //Verifies the password by comparing the derived key with the stored hash
    public static bool VerifyPassword(string MasterPassword, byte [] salt, byte[] hash)
    {
        byte[] candidateKey = DeriveKey(MasterPassword, salt);
        
        return CryptographicOperations.FixedTimeEquals(candidateKey, hash);
    }

    //Creates a new key and returns it
    public static byte[] CreateVaultKey(string masterPassword, byte[] salt)
    {
        return DeriveKey(masterPassword, salt);
    }

    //Encrypts a serverice password and returns it
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

    //Decrypts a service password and returns it
    public static byte[] Decrypt(byte[] encryptedText, byte[] key, byte[] iV)
    {
        int cyphertextLength = encryptedText.Length - tagSize;
        byte[] ciphertext = new byte[cyphertextLength];
        byte[] tag = new byte[tagSize];

        Buffer.BlockCopy(encryptedText, 0, ciphertext, 0, ciphertext.Length);
        Buffer.BlockCopy(encryptedText, ciphertext.Length, tag, 0, tag.Length);

        byte[] plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(key, tagSize);
        aes.Decrypt(iV, ciphertext, tag, plaintext);

        return plaintext;
    }
}