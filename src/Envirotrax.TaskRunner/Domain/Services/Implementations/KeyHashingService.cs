using System.Security.Cryptography;
using Envirotrax.TaskRunner.Domain.Services.Definitions;

namespace Envirotrax.TaskRunner.Domain.Services.Implementations;

public class KeyHashingService : IKeyHashingService
{
    public string GenerateApiKey()
    {
        byte[] keyBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(keyBytes);
    }

    public string HashText(string text)
    {
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        byte[] hashBytes = SHA256.HashData(textBytes);
        return Convert.ToHexString(hashBytes);
    }

    public bool VerifyHashedText(string text, string hash)
    {
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);
        byte[] hashBytes = SHA256.HashData(textBytes);
        byte[] storedHashBytes = Convert.FromHexString(hash);
        return CryptographicOperations.FixedTimeEquals(hashBytes, storedHashBytes);
    }
}
