namespace Envirotrax.TaskRunner.Domain.Services.Definitions;

public interface IKeyHashingService
{
    string GenerateApiKey();
    string HashText(string text);
    bool VerifyHashedText(string text, string hash);
}
