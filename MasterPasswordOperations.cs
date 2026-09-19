public class MasterPasswordServices
{
    public InitialiseMasterPassword(string masterPassword)
    {
        byte[] salt = cryptography.GenerateNewSalt();
        byte[] derivedKey = cryptography.DeriveKey(masterPassword, salt);

        datebase.Add(
            new MasterPasswordEntry
            {
                Salt = salt,
                Hash = derivedKey
            }
        );
    }
    
    
    
    public ChangeMasterPassword(string oldMasterPassword, string newMasterPassword)
    {
        InitializeMasterPassword(newMasterPassword);
    }








}