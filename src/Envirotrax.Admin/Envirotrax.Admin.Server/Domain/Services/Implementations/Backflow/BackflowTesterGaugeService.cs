using System.Globalization;
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Backflow;

public class BackflowTesterGaugeService : IBackflowTesterGaugeService
{
    private const string BaseUrl = "/api/admin/backflow/testers";

    private readonly IEnvirotraxApiClient _apiClient;

    public BackflowTesterGaugeService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<IPagedData<BackflowGaugeDto>> GetAllAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<BackflowGaugeDto>($"{BaseUrl}/{professionalId}/gauges", pageInfo, query, cancellationToken);
    }

    public Task<BackflowGaugeDto?> AddAsync(int professionalId, BackflowGaugeDto gauge, Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        // The calibration certificate is posted as multipart form data, so the scalar fields travel alongside the
        // file rather than as JSON. The App's form DTO requires the owning professional, so it travels too.
        var formFields = new Dictionary<string, string>
        {
            ["professional.id"] = professionalId.ToString(),
            ["manufacturer"] = gauge.Manufacturer,
            ["model"] = gauge.Model,
            ["serialNumber"] = gauge.SerialNumber,
            ["isPortable"] = gauge.IsPortable.ToString()
        };

        if (gauge.LastCalibrationDate.HasValue)
        {
            formFields["lastCalibrationDate"] = gauge.LastCalibrationDate.Value.ToString("O", CultureInfo.InvariantCulture);
        }

        return _apiClient.PostFileAsync<BackflowGaugeDto>(
            $"{BaseUrl}/{professionalId}/gauges",
            fileStream,
            fileName,
            "file",
            formFields,
            cancellationToken);
    }

    public Task<BackflowGaugeDto?> UpdateAsync(int professionalId, int gaugeId, BackflowGaugeDto gauge, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<BackflowGaugeDto, BackflowGaugeDto>($"{BaseUrl}/{professionalId}/gauges/{gaugeId}", gauge, cancellationToken);
    }

    public Task<string?> GetFileUrlAsync(int professionalId, int gaugeId, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<string>($"{BaseUrl}/{professionalId}/gauges/{gaugeId}/file-url", cancellationToken);
    }

    public Task DeleteAsync(int professionalId, int gaugeId, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{BaseUrl}/{professionalId}/gauges/{gaugeId}", cancellationToken);
    }
}
