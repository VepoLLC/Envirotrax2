using Envirotrax.Admin.Server.Domain.DataTransferObjects.Letters;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Letters;

public interface ILetterGeneratorService
{
    Task<GeneratedLettersDto?> GenerateAsync(int waterSupplierId);
}
