
using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalUserRepository : IRepository<ProfessionalUser>
{
    Task<UpdateResult<ProfessionalUser>> UpdateNonSensitiveDataAsync(ProfessionalUser user);
    Task<UpdateResult<ProfessionalUser>> UpdateSignaturePathAsync(int userId, string signaturePath);
    Task<UpdateResult<ProfessionalUser>> UpdateSubAccountAsync(int professionalId, int userId, string? contactName, string? jobTitle);
    Task<IEnumerable<ProfessionalUser>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, Expression<Func<ProfessionalUser, bool>>? roleFilter = null);
    Task<IEnumerable<ProfessionalUser>> SearchCsiInspectorsAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, CancellationToken cancellationToken);
}