namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IKeyHashingService
{
    string GenerateApiKey();
    string HashText(string text);
    bool VerifyHashedText(string text, string hash);
}
