using System.Globalization;
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

public class FogInspectorInsuranceService : IFogInspectorInsuranceService
{
    private const string BaseUrl = "/api/admin/fog/inspectors";

    private readonly IEnvirotraxApiClient _apiClient;

    public FogInspectorInsuranceService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<ProfessionalInsuranceDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalInsuranceDto>($"{BaseUrl}/{professionalId}/insurances", pageInfo, query, cancellationToken);
    }

    public Task<ProfessionalInsuranceDto?> AddAsync(int professionalId, ProfessionalInsuranceDto insurance, Stream fileStream, string fileName, CancellationToken cancellationToken)
    {

        var formFields = new Dictionary<string, string>
        {
            ["professional.id"] = professionalId.ToString(),
            ["insuranceNumber"] = insurance.InsuranceNumber
        };

        if (insurance.CoverageAmount.HasValue)
        {
            formFields["coverageAmount"] = insurance.CoverageAmount.Value.ToString(CultureInfo.InvariantCulture);
        }

        if (insurance.ExpirationDate.HasValue)
        {
            formFields["expirationDate"] = insurance.ExpirationDate.Value.ToString("O", CultureInfo.InvariantCulture);
        }

        return _apiClient.PostFileAsync<ProfessionalInsuranceDto>(
            $"{BaseUrl}/{professionalId}/insurances",
            fileStream,
            fileName,
            "file",
            formFields,
            cancellationToken);
    }

    public Task<ProfessionalInsuranceDto?> UpdateAsync(int professionalId, int insuranceId, ProfessionalInsuranceDto insurance, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<ProfessionalInsuranceDto, ProfessionalInsuranceDto>($"{BaseUrl}/{professionalId}/insurances/{insuranceId}", insurance, cancellationToken);
    }

    public Task<string?> GetFileUrlAsync(int professionalId, int insuranceId, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<string>($"{BaseUrl}/{professionalId}/insurances/{insuranceId}/file-url", cancellationToken);
    }

    public Task DeleteAsync(int professionalId, int insuranceId, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{professionalId}/insurances/{insuranceId}", cancellationToken);
    }
}
