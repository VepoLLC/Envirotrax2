
using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IEmailService
{
    Task SendAsync(EmailDto email);
    Task SendAsync<TTemplate>(EmailDto<TTemplate> email);
}