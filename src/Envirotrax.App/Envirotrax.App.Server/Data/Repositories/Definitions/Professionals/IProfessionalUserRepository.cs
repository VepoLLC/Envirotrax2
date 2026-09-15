
using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalUserRepository : IRepository<ProfessionalUser>
{
    Task<UpdateResult<ProfessionalUser>> UpdateNonSensitiveDataAsync(ProfessionalUser user);
    Task<UpdateResult<ProfessionalUser>> UpdateSignaturePathAsync(int userId, string signaturePath);
    Task<UpdateResult<ProfessionalUser>> UpdateSubAccountAsync(int professionalId, int userId, string? contactName, string? jobTitle);
    Task<IEnumerable<ProfessionalUser>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, Expression<Func<ProfessionalUser, bool>>? roleFilter = null);
    Task<IEnumerable<ProfessionalUser>> SearchAccountsAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, Expression<Func<ProfessionalUserLicense, bool>> licenseFilter, Expression<Func<ProfessionalUser, bool>> roleFilter, CancellationToken cancellationToken);
}