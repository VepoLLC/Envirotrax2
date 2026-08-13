
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
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

    public Task<IPagedData<ProfessionalDto>> SearchAsync(PageInfo pageInfo, Query query, string? inspectorLicenseNumber, string? insurancePolicyNumber, string? userEmail, string? contactName, CancellationToken cancellationToken)
    {
        var additionalParameters = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(inspectorLicenseNumber))
        {
            additionalParameters["inspectorLicenseNumber"] = inspectorLicenseNumber;
        }

        if (!string.IsNullOrWhiteSpace(insurancePolicyNumber))
        {
            additionalParameters["insurancePolicyNumber"] = insurancePolicyNumber;
        }

        if (!string.IsNullOrWhiteSpace(userEmail))
        {
            additionalParameters["userEmail"] = userEmail;
        }

        if (!string.IsNullOrWhiteSpace(contactName))
        {
            additionalParameters["contactName"] = contactName;
        }

        return _apiClient.GetAsync<ProfessionalDto>(BaseUrl, pageInfo, query, additionalParameters, cancellationToken);
    }
}
