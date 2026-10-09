using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Sites;

public interface IMailingInfoRedactionService
{
    Task<TDto> RedactAsync<TDto>(TDto dto, CancellationToken cancellationToken)
        where TDto : IRedactableMailingInfoDto;

    Task<List<TDto>> RedactAsync<TDto>(IEnumerable<TDto> dtos, CancellationToken cancellationToken)
        where TDto : IRedactableMailingInfoDto;

    Task<IPagedData<TDto>> RedactAsync<TDto>(IPagedData<TDto> page, CancellationToken cancellationToken)
        where TDto : IRedactableMailingInfoDto;
}
