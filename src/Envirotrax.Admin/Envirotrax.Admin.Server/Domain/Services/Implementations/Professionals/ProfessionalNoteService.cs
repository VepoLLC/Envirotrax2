using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;

namespace Envirotrax.Admin.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalNoteService : IProfessionalNoteService
{
    private readonly IEnvirotraxApiClient _apiClient;

    public ProfessionalNoteService(IEnvirotraxApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private static string GetNotesUrl(ProfessionalType professionalType, int professionalId)
    {
        var professionalsUrl = professionalType switch
        {
            ProfessionalType.Bpat => "/api/admin/backflow/testers",
            ProfessionalType.CsiInspector => "/api/admin/csi/inspectors",
            ProfessionalType.FogInspector => "/api/admin/fog/inspectors",
            ProfessionalType.FogTransporter => "/api/admin/fog/transporters",
            _ => throw new ArgumentOutOfRangeException(nameof(professionalType), professionalType, null)
        };

        return $"{professionalsUrl}/{professionalId}/notes";
    }

    public Task<IPagedData<ProfessionalNoteDto>> GetAllAsync(ProfessionalType professionalType, int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _apiClient.GetAsync<ProfessionalNoteDto>(GetNotesUrl(professionalType, professionalId), pageInfo, query, cancellationToken);
    }

    public Task<ProfessionalNoteDto?> AddAsync(ProfessionalType professionalType, int professionalId, ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        return _apiClient.PostAsync<ProfessionalNoteDto, ProfessionalNoteDto>(GetNotesUrl(professionalType, professionalId), note, cancellationToken);
    }

    public Task<ProfessionalNoteDto?> UpdateAsync(ProfessionalType professionalType, int professionalId, int noteId, ProfessionalNoteDto note, CancellationToken cancellationToken)
    {
        return _apiClient.PutAsync<ProfessionalNoteDto, ProfessionalNoteDto>($"{GetNotesUrl(professionalType, professionalId)}/{noteId}", note, cancellationToken);
    }

    public Task DeleteAsync(ProfessionalType professionalType, int professionalId, int noteId, CancellationToken cancellationToken)
    {
        return _apiClient.DeleteAsync<object>($"{GetNotesUrl(professionalType, professionalId)}/{noteId}", cancellationToken);
    }
}
