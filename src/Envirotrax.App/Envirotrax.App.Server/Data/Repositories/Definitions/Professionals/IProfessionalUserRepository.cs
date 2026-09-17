
using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;

public interface IProfessionalUserRepository : IRepository<ProfessionalUser>
{
    Task<ProfessionalUser?> UpdateNonSensitiveDataAsync(ProfessionalUser user);
    Task<ProfessionalUser?> UpdateSignaturePathAsync(int userId, string signaturePath);
    Task<ProfessionalUser?> UpdateSubAccountAsync(int professionalId, int userId, string? contactName, string? jobTitle);
    Task<IEnumerable<ProfessionalUser>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, Expression<Func<ProfessionalUser, bool>>? roleFilter = null);
    Task<IEnumerable<ProfessionalUser>> SearchCsiInspectorsAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, CancellationToken cancellationToken);
}