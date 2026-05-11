using System.Transactions;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;
using Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;

namespace Envirotrax.App.Server.Domain.Services.Implementations.GisAreas;

public class GisAreaCoordinateService : Service<GisAreaCoordinate, GisAreaCoordinateDto, long>, IGisAreaCoordinateService
{
    private readonly IGisAreaCoordinateRepository _coordinateRepository;
    private readonly IGisAreaRepository _areaRepository;

    public GisAreaCoordinateService(
        IMapper mapper,
        IGisAreaCoordinateRepository repository,
        IGisAreaRepository gisAreaRepository)
        : base(mapper, repository)
    {
        _coordinateRepository = repository;
        _areaRepository = gisAreaRepository;
    }

    public async Task<IEnumerable<GisAreaCoordinateDto>> GetByAreaIdAsync(int areaId, CancellationToken cancellationToken)
    {
        var models = await _coordinateRepository.GetByAreaIdAsync(areaId, cancellationToken);
        return Mapper.Map<IEnumerable<GisAreaCoordinate>, IEnumerable<GisAreaCoordinateDto>>(models);
    }

    public async Task<IEnumerable<GisAreaCoordinateDto>> AddOrUpdateAsync(int areaId, IEnumerable<GisAreaCoordinateDto> coordinates)
    {
        var model = Mapper.Map<IEnumerable<GisAreaCoordinateDto>, IEnumerable<GisAreaCoordinate>>(coordinates);

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var added = await _coordinateRepository.AddOrUpdateAsync(areaId, model);

        await _areaRepository.UpdateBoundsAsync(
            areaId,
            added.Min(c => c.Longitude),
            added.Min(c => c.Latitude),
            added.Max(c => c.Longitude),
            added.Max(c => c.Latitude));

        scope.Complete();

        return Mapper.Map<IEnumerable<GisAreaCoordinate>, IEnumerable<GisAreaCoordinateDto>>(added);
    }

    public async Task DeleteByAreaAsync(int areaId)
    {
        await _coordinateRepository.DeleteByAreaAsyc(areaId);
    }
}
