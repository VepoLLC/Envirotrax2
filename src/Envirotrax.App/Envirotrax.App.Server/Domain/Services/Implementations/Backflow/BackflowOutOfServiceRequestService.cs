using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowOutOfServiceRequestService : Service<BackflowOutOfServiceRequest, BackflowOutOfServiceRequestDto>, IBackflowOutOfServiceRequestService
{
    private readonly IBackflowOutOfServiceRequestRepository _repository;
    private readonly IAuthService _authService;

    public BackflowOutOfServiceRequestService(
        IMapper mapper,
        IBackflowOutOfServiceRequestRepository repository,
        IAuthService authService)
        : base(mapper, repository)
    {
        _repository = repository;
        _authService = authService;
    }

    public async Task<BackflowOutOfServiceRequestDto> SubmitAsync(BackflowOutOfServiceRequestDto dto, CancellationToken cancellationToken)
    {
        dto.Id = 0;
        dto.Professional = new ReferencedProfessionalDto { Id = _authService.ProfessionalId };
        dto.BpatId = _authService.UserId;

        if (dto.Type != OutOfServiceType.Replaced && dto.Type != OutOfServiceType.Removed)
        {
            throw new ValidationException("A valid out-of-service reason is required.");
        }

        if (dto.Type == OutOfServiceType.Replaced && !dto.ReplacementAssemblyTestId.HasValue)
        {
            throw new ValidationException("A replacement assembly test is required when the assembly is replaced.");
        }

        if (dto.Type == OutOfServiceType.Removed && string.IsNullOrWhiteSpace(dto.Description))
        {
            throw new ValidationException("A description is required when the assembly is removed.");
        }

        // The request belongs to the source test's water supplier (as in V1). This also supplies the
        // WaterSupplierId the tenant key needs, since the professional context does not auto-set it.
        var testWaterSupplierId = await _repository.GetTestWaterSupplierIdAsync(dto.TestId, cancellationToken);
        if (testWaterSupplierId == null)
        {
            throw new ValidationException("The selected backflow test could not be found.");
        }

        dto.WaterSupplier = new ReferencedWaterSupplierDto { Id = testWaterSupplierId.Value };

        // V1 blocks a test that already has an out-of-service request.
        if (await _repository.HasRequestForTestAsync(dto.TestId, cancellationToken))
        {
            throw new ValidationException("The assembly has already been requested to be marked as out of service.");
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            dto.Description = dto.Type == OutOfServiceType.Replaced
                ? "The assembly was replaced by another assembly"
                : "The assembly was removed";
        }

        dto.OutOfServiceDate = null;
        dto.ClearedDate = null;

        return await base.AddAsync(dto);
    }

    public async Task<IEnumerable<BackflowTestDto>> GetReplacementCandidatesAsync(int testId, CancellationToken cancellationToken)
    {
        var candidates = await _repository.GetReplacementCandidatesAsync(testId, cancellationToken);
        return candidates.Select(c => Mapper.Map<BackflowTestDto>(c)!);
    }
}
