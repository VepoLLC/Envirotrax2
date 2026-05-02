
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowGaugeService : IService<BackflowGaugeDto>
{
    Task<BackflowGaugeDto> AddWithFileAsync(Stream fileStream, string originalFileName, BackflowGaugeDto dto);
}
