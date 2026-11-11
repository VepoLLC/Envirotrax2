using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Licenses;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Licenses;

public class InsuranceService : IInsuranceService
{
    private const string BaseUrl = "/api/admin/insurances";

    private readonly IEnvirotraxApiClient _apiClient;

    public InsuranceService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<WaterSupplierInsuranceDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<WaterSupplierInsuranceDto>(BaseUrl, pageInfo, query, cancellationToken);
    }

    public Task<ProfessionalInsuranceDto?> UpdateAsync(int id, ProfessionalInsuranceDto insurance)
    {
        return _apiClient.PutAsync<ProfessionalInsuranceDto, ProfessionalInsuranceDto>($"{BaseUrl}/{id}", insurance, CancellationToken.None);
    }

    public Task<string?> GetFileUrlAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<string>($"{BaseUrl}/{id}/file-url", cancellationToken);
    }

    public Task<List<RecordLogDto>?> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<List<RecordLogDto>>($"{BaseUrl}/{id}/logs", cancellationToken);
    }
}
