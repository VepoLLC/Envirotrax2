using AutoMapper;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiInspectionAssemblyService : ICsiInspectionAssemblyService
{
    private readonly IMapper _mapper;
    private readonly ICsiInspectionAssemblyRepository _repository;

    public CsiInspectionAssemblyService(IMapper mapper, ICsiInspectionAssemblyRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<List<CsiInspectionAssemblyDto>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        var assemblies = await _repository.GetByInspectionAsync(inspectionId, cancellationToken);

        return assemblies
            .Select(assembly => _mapper.Map<CsiInspectionAssemblyDto>(assembly))
            .ToList();
    }

    public Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        return _repository.GetCountByInspectionAsync(inspectionId, cancellationToken);
    }
}
