using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog
{
    public class FogTransporterRepository : Repository<Professional>, IFogTransporterRepository
    {
        public FogTransporterRepository(IDbContextSelector dbContextSelector)
            : base(dbContextSelector)
        {
        }

        protected override IQueryable<Professional> GetListQuery()
        {
            return Entity.AsNoTracking()
            .Include(p => p.State)
            .Where(p => p.HasFogTransportation);
        }

        public async Task<IEnumerable<Professional>> SearchAsync(FogTransporterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
        {
            var dbQuery = GetListQuery()
                .Where(query.Filter)
                .WhereIf(!string.IsNullOrWhiteSpace(criteria.RegistrationNumber), p => DbContext.ProfessionalLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.FogTransporter &&
                    l.LicenseNumber.Contains(criteria.RegistrationNumber!)))
                .WhereIf(!string.IsNullOrWhiteSpace(criteria.InsurancePolicyNumber), p => DbContext.ProfessionalInsurances.Any(i =>
                    i.ProfessionalId == p.Id &&
                    i.InsuranceNumber.Contains(criteria.InsurancePolicyNumber!)))
                .WhereIf(!string.IsNullOrWhiteSpace(criteria.UserEmail), p =>
                    p.CompanyEmail!.Contains(criteria.UserEmail!) ||
                    DbContext.ProfessionalUsers.Any(u =>
                        u.ProfessionalId == p.Id &&
                        u.IsFogTransporter &&
                        u.User!.Email!.Contains(criteria.UserEmail!)))
                .WhereIf(!string.IsNullOrWhiteSpace(criteria.ContactName), p => DbContext.ProfessionalUsers.Any(u =>
                    u.ProfessionalId == p.Id &&
                    u.IsFogTransporter &&
                    u.ContactName!.Contains(criteria.ContactName!)))
                .WhereIf(!string.IsNullOrWhiteSpace(criteria.CellNumber), p =>
                    p.CellPhoneNumber!.Contains(criteria.CellNumber!) ||
                    DbContext.ProfessionalUsers.Any(u =>
                        u.ProfessionalId == p.Id &&
                        u.IsFogTransporter &&
                        u.User!.PhoneNumber!.Contains(criteria.CellNumber!)));

            if (query.Sort.IsNullOrEmpty())
            {
                query.Sort[nameof(Professional.Name)] = SortOperator.Asc;
            }

            var paginated = await dbQuery
                .OrderBy(query.Sort)
                .PaginateAsync(pageInfo, cancellationToken);

            return await paginated.ToListAsync(cancellationToken);
        }
    }
}
