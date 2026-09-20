public class Program
{
    public static void Main(string[] args)
    {
        using var database = new AppDbContext();

        database.Database.EnsureCreated();

        var appInitialisation = new AppInitialisation(database);
        appInitialisation.InitialiseOrUnlockVault();
        }
    }