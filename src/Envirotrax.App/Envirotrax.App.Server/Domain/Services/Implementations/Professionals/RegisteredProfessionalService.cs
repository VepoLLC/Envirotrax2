
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class RegisteredProfessionalService : IRegisteredProfessionalService
{
    /// <summary>
    /// The account types the public directory publishes. Anything else is rejected before it can
    /// reach a query, so the anonymous endpoint cannot be used to enumerate other professional types.
    /// </summary>
    private static readonly ProfessionalType[] PublishedTypes =
    [
        ProfessionalType.Bpat,
        ProfessionalType.CsiInspector,
        ProfessionalType.FogInspector,
        ProfessionalType.FogTransporter
    ];

    private readonly IMapper _mapper;
    private readonly IRegisteredProfessionalRepository _registeredProfessionalRepository;

    public RegisteredProfessionalService(IMapper mapper, IRegisteredProfessionalRepository repository)
    {
        _mapper = mapper;
        _registeredProfessionalRepository = repository;
    }

    public async Task<IReadOnlyList<RegisteredProfessionalSupplierDto>> GetWaterSuppliersAsync(
        ProfessionalType professionalType,
        CancellationToken cancellationToken)
    {
        if (!IsPublished(professionalType))
        {
            return [];
        }

        var suppliers = await _registeredProfessionalRepository.GetWaterSuppliersAsync(professionalType, cancellationToken);

        return [.. _mapper.Map<IEnumerable<RegisteredProfessionalSupplierDto>>(suppliers)];
    }

    public async Task<IPagedData<RegisteredProfessionalDto>> SearchAsync(
        int waterSupplierId,
        ProfessionalType professionalType,
        PageInfo pageInfo,
        Query query,
        CancellationToken cancellationToken)
    {
        if (!IsPublished(professionalType) || waterSupplierId <= 0)
        {
            return Enumerable.Empty<RegisteredProfessionalDto>().ToPagedData(pageInfo);
        }

        query.Filter = query.ConvertFilterProperties<RegisteredProfessional, RegisteredProfessionalDto>(_mapper);
        query.Sort = query.ConvertSortProperties<RegisteredProfessional, RegisteredProfessionalDto>(_mapper);

        var results = await _registeredProfessionalRepository.SearchAsync(
            waterSupplierId, professionalType, pageInfo, query, cancellationToken);

        return _mapper
            .Map<IEnumerable<RegisteredProfessional>, IEnumerable<RegisteredProfessionalDto>>(results)
            .ToPagedData(pageInfo);
    }

    private static bool IsPublished(ProfessionalType professionalType)
    {
        return PublishedTypes.Contains(professionalType);
    }
}
