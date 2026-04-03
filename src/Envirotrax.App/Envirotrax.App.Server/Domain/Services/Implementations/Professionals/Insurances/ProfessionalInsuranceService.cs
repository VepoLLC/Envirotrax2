
using System.Transactions;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals.Insurances;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Insurances;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Insurances;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Insurances;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals.Insurances;

public class ProfessionalInsuranceService : Service<ProfessionalInsurance, ProfessionalInsuranceDto>, IProfessionalInsuranceService
{
    private readonly IFileStorageService _fileStorageService;

    public ProfessionalInsuranceService(
        IMapper mapper,
        IProfessionalInsuranceRepository repository,
        IFileStorageService fileStorageService)
        : base(mapper, repository)
    {
        _fileStorageService = fileStorageService;
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
}