using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

public class FogTransporterVehicleService : IFogTransporterVehicleService
{
    private const string BaseUrl = "/api/admin/fog/transporters";

    private readonly IEnvirotraxApiClient _apiClient;

    public FogTransporterVehicleService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<FogVehicleDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<FogVehicleDto>($"{BaseUrl}/{professionalId}/vehicles", pageInfo, query, cancellationToken);
    }

    public Task<FogVehicleDto?> AddAsync(int professionalId, FogVehicleDto vehicle, CancellationToken cancellationToken)
    {
        return _apiClient.PostAsync<FogVehicleDto, FogVehicleDto>($"{BaseUrl}/{professionalId}/vehicles", vehicle, cancellationToken);
    }

    public Task<FogVehicleDto?> UpdateAsync(int professionalId, int vehicleId, FogVehicleDto vehicle, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<FogVehicleDto, FogVehicleDto>($"{BaseUrl}/{professionalId}/vehicles/{vehicleId}", vehicle, cancellationToken);
    }

    public Task DeleteAsync(int professionalId, int vehicleId, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{professionalId}/vehicles/{vehicleId}", cancellationToken);
    }
}
