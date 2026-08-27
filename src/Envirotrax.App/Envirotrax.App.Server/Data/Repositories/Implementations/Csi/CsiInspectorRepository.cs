using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi
{
    public class CsiInspectorRepository : Repository<Professional>, ICsiInspectorRepository
    {
        public CsiInspectorRepository(IDbContextSelector dbContextSelector)
            : base(dbContextSelector)
        {
        }

        protected override IQueryable<Professional> GetListQuery()
        {
            return base.GetListQuery()
                .Include(p => p.State);
        }

        protected override IQueryable<Professional> GetDetailsQuery()
        {
            return Entity.AsNoTracking().Include(p => p.State);
        }

        public async Task<IEnumerable<Professional>> SearchAsync(string? inspectorLicenseNumber, string? insurancePolicyNumber, string? userEmail, string? contactName, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
        {
            var dbQuery = GetListQuery()
                .Where(query.Filter);

            if (!string.IsNullOrWhiteSpace(inspectorLicenseNumber))
            {
                dbQuery = dbQuery.Where(p => DbContext.ProfessionalUserLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.CsiInspector &&
                    l.LicenseNumber.Contains(inspectorLicenseNumber)));
            }

            if (!string.IsNullOrWhiteSpace(insurancePolicyNumber))
            {
                dbQuery = dbQuery.Where(p => DbContext.ProfessionalInsurances.Any(i =>
                    i.ProfessionalId == p.Id &&
                    i.InsuranceNumber.Contains(insurancePolicyNumber)));
            }

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                dbQuery = dbQuery.Where(p =>
                    p.CompanyEmail!.Contains(userEmail) ||
                    DbContext.ProfessionalUsers.Any(u =>
                        u.ProfessionalId == p.Id &&
                        u.IsCsiInspector &&
                        u.User!.Email!.Contains(userEmail)));
            }

            if (!string.IsNullOrWhiteSpace(contactName))
            {
                dbQuery = dbQuery.Where(p => DbContext.ProfessionalUsers.Any(u =>
                    u.ProfessionalId == p.Id &&
                    u.IsCsiInspector &&
                    u.ContactName!.Contains(contactName)));
            }

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
