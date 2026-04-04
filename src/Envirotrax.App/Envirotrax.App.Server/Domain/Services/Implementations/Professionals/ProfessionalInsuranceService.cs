
using System.Transactions;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalInsuranceService : Service<ProfessionalInsurance, ProfessionalInsuranceDto>, IProfessionalInsuranceService
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public ProfessionalInsuranceService(
        IMapper mapper,
        IProfessionalInsuranceRepository repository,
        IFileStorageService fileStorageService,
        ITimeZoneHelperService timeZoneHelper)
        : base(mapper, repository)
    {
        _fileStorageService = fileStorageService;
        _timeZoneHelper = timeZoneHelper;
    }


    protected override ProfessionalInsuranceDto? MapToDto(ProfessionalInsurance? model)
    {
        var dto = base.MapToDto(model);

        if (dto != null)
        {
            var localTime = _timeZoneHelper.GetUserLocalTime();

            if (localTime > dto.ExpirationDate)
            {
                dto.ExpirationType = ExpirationType.Expired;
            }
            else if (localTime.AddDays(-30) <= dto.ExpirationDate)
            {
                dto.ExpirationType = ExpirationType.AboutToExpire;
            }
        }

        return dto;
    }

    public async Task<ProfessionalInsuranceDto> AddAsync(Stream fileStream, string originalFileName, ProfessionalInsuranceDto insurance)
    {
        var fileExtension = Path.GetExtension(originalFileName);
        insurance.FilePath = $"insurances/{Guid.NewGuid()}{fileExtension}";

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var added = await AddAsync(insurance);
            await _fileStorageService.UploadAsync(insurance.FilePath, fileStream);

            scope.Complete();
            return added;
        }
    }

    public override async Task<ProfessionalInsuranceDto?> DeleteAsync(int id)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var deleted = await base.DeleteAsync(id);

            if (deleted != null)
            {
                await _fileStorageService.DeleteAsync(deleted.FilePath!);
                scope.Complete();
            }

            return deleted;
        }
    }
}