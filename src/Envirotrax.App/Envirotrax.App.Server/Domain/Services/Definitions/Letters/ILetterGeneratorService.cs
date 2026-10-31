using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Letters;

public interface ILetterGeneratorService
{
    Task<GeneratedLettersDto?> GenerateAsync(int waterSupplierId);
}
