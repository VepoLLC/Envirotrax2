using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowTestService : Service<BackflowTest, BackflowTestDto>, IBackflowTestService
{
    private readonly IBackflowTestRepository _backflowTestRepository;
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;

    public BackflowTestService(
        IMapper mapper,
        IBackflowTestRepository repository,
        IProfessionalRepository professionalRepository,
        IProfessionalUserRepository professionalUserRepository)
        : base(mapper, repository)
    {
        _backflowTestRepository = repository;
        _professionalRepository = professionalRepository;
        _professionalUserRepository = professionalUserRepository;
    }

    public override async Task<IPagedData<BackflowTestDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var gisAreaFilter = query.Filter.Find(f =>
            string.Equals(f.ColumnName, "gisAreaId", StringComparison.OrdinalIgnoreCase));

        int? gisAreaId = null;
        if (gisAreaFilter != null)
        {
            query.Filter.Remove(gisAreaFilter);
            if (int.TryParse(gisAreaFilter.Value?.ToString(), out var parsed))
            {
                gisAreaId = parsed;
            }
        }

        query.Sort = query.ConvertSortProperties<BackflowTest, BackflowTestDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<BackflowTest, BackflowTestDto>(Mapper);

        var model = await _backflowTestRepository.GetAllAsync(pageInfo, query, gisAreaId, cancellationToken);
        return model.Select(m => MapToDto(m)!).ToPagedData(pageInfo);
    }

    public override async Task<BackflowTestDto> AddAsync(BackflowTestDto dto)
    {
        await PopulateBpatSnapshotAsync(dto);
        return await base.AddAsync(dto);
    }

    private async Task PopulateBpatSnapshotAsync(BackflowTestDto dto)
    {
        if (dto.Professional?.Id is int professionalId)
        {
            var professional = await _professionalRepository.GetAsync(professionalId, CancellationToken.None);
            if (professional != null)
            {
                dto.BpatCompanyName = professional.Name;
                dto.BpatAddress = professional.Address;
                dto.BpatCity = professional.City;
                dto.BpatZip = professional.ZipCode;
                dto.BpatWorkNumber = professional.PhoneNumber;
                if (professional.StateId.HasValue)
                {
                    dto.BpatState = new ReferencedStateDto { Id = professional.StateId.Value };
                }
            }
        }

        if (dto.Bpat?.Id is int bpatId)
        {
            var bpatUser = await _professionalUserRepository.GetAsync(bpatId, CancellationToken.None);
            if (bpatUser != null)
            {
                dto.BpatContactName = bpatUser.ContactName;
            }
        }
    }
}
