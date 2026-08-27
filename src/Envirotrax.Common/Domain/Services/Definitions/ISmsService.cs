
using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface ISmsService
{
    Task SendAsync(SmsDto sms);
}