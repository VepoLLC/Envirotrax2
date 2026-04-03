
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Insurances;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Insurances;

public interface IProfessionalInsuranceService : IService<ProfessionalInsuranceDto>
{
    Task<ProfessionalInsuranceDto> AddAsync(Stream fileStream, string originalFileName, ProfessionalInsuranceDto insurance);
}