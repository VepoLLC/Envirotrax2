using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowTestService : IService<BackflowTest, BackflowTestDto>
{
    Task<BackflowTestDto> SubmitWithImagesAsync(
        BackflowTestDto dto,
        Stream? assemblyStream, string? assemblyFileName,
        Stream? serialStream, string? serialFileName,
        Stream? bypassAssemblyStream, string? bypassAssemblyFileName,
        Stream? bypassSerialStream, string? bypassSerialFileName,
        Stream? airGapStream, string? airGapFileName,
        CancellationToken cancellationToken = default);
}
