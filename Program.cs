//start
//checks if vault is created, if not it will create a new vault and ask the user to create a master password
//when the user creates a master password, it will generate and store a new salt and derive a key from the master password and salt
//if vault is created, it will ask the user to enter the master password and verify it against the stored hash and salt

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        using var database = new AppDbContext();

        database.Database.EnsureCreated();

        bool vaultExists = database.VaultMetadata.Any();

        if (!vaultExists)
        {
            string masterPassword = ReadPassword("Create a new master password: ");
            var masterPasswordServices = new MasterPasswordServices(database);
            masterPasswordServices.InitialiseMasterPassword(masterPassword);
        }
        else
        {
            string masterPassword = ReadPassword("Enter master password:  ");
            if (Cryptography.VerifyPassword(masterPassword, database.VaultMetadata.First().Salt, database.VaultMetadata.First().PasswordVerificationHash))
            {
                Console.WriteLine("Master password verified successfully.");
                //decrypt the vault and allow the user to access the password manager
            }
            else
            {
                Console.WriteLine("Incorrect master password.");
            }
        }
    }

    private static string ReadPassword(string prompt)
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
}