using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogInspectionService : Service<FogInspection, FogInspectionDto>, IFogInspectionService
{
    private readonly IFogInspectionRepository _repository;
    private readonly IProfessionalService _professionalService;

    public FogInspectionService(
        IMapper mapper,
        IFogInspectionRepository repository,
        IProfessionalService professionalService)
        : base(mapper, repository)
    {
        _repository = repository;
        _professionalService = professionalService;
    }

    public async Task<IPagedData<FogInspectionDto>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken)
    {
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken)
            ?? throw new InvalidOperationException("Professional not found.");

        var facilityTypeProps = query.Filter
            .Where(f => f.ColumnName?.Equals("facilityType", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        query.Filter.RemoveAll(f =>
            string.Equals(f.ColumnName, "facilityType", StringComparison.OrdinalIgnoreCase));

        query.Filter = query.ConvertFilterProperties<FogInspection, FogInspectionDto>(Mapper);
        query.Sort = query.ConvertSortProperties<FogInspection, FogInspectionDto>(Mapper);

        var facilityTypes = facilityTypeProps
            .SelectMany(f => f.Children ?? Enumerable.Empty<QueryProperty>())
            .Where(c => int.TryParse(c.Value?.ToString(), out _))
            .Select(c => (FacilityType)int.Parse(c.Value!.ToString()!))
            .Distinct().ToList();

        var inspections = await _repository.SearchForProfessionalAsync(
            professional.Id, pageInfo, query, facilityTypes, latestOnly, cancellationToken);

        return inspections.Select(m => Mapper.Map<FogInspectionDto>(m)!).ToPagedData(pageInfo);
    }
}
