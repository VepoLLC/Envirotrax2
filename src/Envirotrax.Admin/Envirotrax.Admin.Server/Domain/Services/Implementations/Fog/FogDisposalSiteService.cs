
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

public class FogDisposalSiteService : IFogDisposalSiteService
{
    private const string BaseUrl = "/api/admin/fog/disposal-sites";

    private readonly IEnvirotraxApiClient _apiClient;

    public FogDisposalSiteService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<FogDisposalSiteDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<FogDisposalSiteDto>(BaseUrl, pageInfo, query, cancellationToken);
    }

    // Disposal sites are not owned by a water supplier, so they use the tenant-less write overloads.
    public Task<FogDisposalSiteDto?> AddAsync(FogDisposalSiteDto site, CancellationToken cancellationToken)
    {
        return _apiClient.PostAsync<FogDisposalSiteDto, FogDisposalSiteDto>(BaseUrl, site, cancellationToken);
    }

    public Task<FogDisposalSiteDto?> UpdateAsync(int id, FogDisposalSiteDto site, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<FogDisposalSiteDto, FogDisposalSiteDto>($"{BaseUrl}/{id}", site, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{id}", cancellationToken);
    }
}
