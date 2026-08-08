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

        public async Task<IEnumerable<Professional>> SearchAsync(string? inspectorLicenseNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken)
        {
            var query = GetListQuery();

            if (!string.IsNullOrWhiteSpace(inspectorLicenseNumber))
            {
                query = query.Where(p => DbContext.ProfessionalUserLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.CsiInspector &&
                    l.LicenseNumber.Contains(inspectorLicenseNumber)));
            }

            if (!string.IsNullOrWhiteSpace(insurancePolicyNumber))
            {
                query = query.Where(p => DbContext.ProfessionalInsurances.Any(i =>
                    i.ProfessionalId == p.Id &&
                    i.InsuranceNumber.Contains(insurancePolicyNumber)));
            }

            var paginated = await query
                .OrderBy(p => p.Name)
                .PaginateAsync(pageInfo, cancellationToken);

            return await paginated.ToListAsync(cancellationToken);
        }
    }
}
