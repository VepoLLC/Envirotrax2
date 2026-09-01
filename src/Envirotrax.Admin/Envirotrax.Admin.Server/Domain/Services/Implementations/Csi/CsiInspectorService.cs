
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Csi;

public class CsiInspectorService : ICsiInspectorService
{
    private const string BaseUrl = "/api/admin/csi/inspectors";

    private readonly IEnvirotraxApiClient _apiClient;

    public CsiInspectorService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<CsiInspectorAccountDto>> SearchAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, CancellationToken cancellationToken)
    {
        var additionalParameters = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(licenseNumber))
        {
            additionalParameters["licenseNumber"] = licenseNumber;
        }

        if (!string.IsNullOrWhiteSpace(insuranceNumber))
        {
            additionalParameters["insuranceNumber"] = insuranceNumber;
        }

        return _apiClient.GetAsync<CsiInspectorAccountDto>(BaseUrl, pageInfo, query, additionalParameters, cancellationToken);
    }

    public Task<CsiInspectorAccountDetailsDto?> GetDetailsAsync(int professionalId, int? userId, CancellationToken cancellationToken)
    {
        var url = userId.HasValue
            ? $"{BaseUrl}/{professionalId}?userId={userId.Value}"
            : $"{BaseUrl}/{professionalId}";

        return _apiClient.GetAsync<CsiInspectorAccountDetailsDto>(url, cancellationToken);
    }

    public Task<CsiInspectorAccountDetailsDto?> UpdateDetailsAsync(int professionalId, CsiInspectorAccountDetailsDto details, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<CsiInspectorAccountDetailsDto, CsiInspectorAccountDetailsDto>($"{BaseUrl}/{professionalId}", details, cancellationToken);
    }
}
