using System.Security.Cryptography;
using System.Text;

namespace DataGateVPNBot.Services.Untils;

public class PasswordGenerator
{
    private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    private const string Digits = "0123456789";
    // private const string SpecialChars = "!@#$%^&*()-_=+<>?";
    private const string SpecialChars = "!@#%&-_+?";
    private static readonly string SecretKey = "MySuperSecretString123!";//todo: get from settings

    public static string GeneratePassword(int length = 16)
    {
        if (length < 4) throw new ArgumentException("Password length must be at least 4 characters.");

        // make hash - key - time - guid
        string hashSeed = GetSha256Hash(SecretKey + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + Guid.NewGuid());
        
        string charPool = Uppercase + Lowercase + Digits + SpecialChars;
        char[] password = new char[length];

        for (int i = 0; i < length; i++)
        {
            int index = hashSeed[i % hashSeed.Length] % charPool.Length;
            password[i] = charPool[index];
        }

        return new string(password);
    }

    private static string GetSha256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes).Replace("=", "").Replace("/", "").Replace("+", "");
        }
    }
}