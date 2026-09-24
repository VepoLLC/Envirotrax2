
namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IRecaptchaVerificationService
{
    Task<bool> VerifyAsync(string token, string? remoteIp = null);
}
