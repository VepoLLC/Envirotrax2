using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog
{
    public class FogInspectorRepository : Repository<Professional>, IFogInspectorRepository
    {
        public FogInspectorRepository(IDbContextSelector dbContextSelector)
            : base(dbContextSelector)
        {
        }

        protected override IQueryable<Professional> GetListQuery()
        {
            return Entity.AsNoTracking()
            .Include(p => p.State)
            .Where(p => p.HasFogInspection);
        }

        public async Task<IEnumerable<Professional>> SearchAsync(string? inspectorLicenseNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken)
        {
            var query = GetListQuery();

            if (!string.IsNullOrWhiteSpace(inspectorLicenseNumber))
            {
                query = query.Where(p => DbContext.ProfessionalUserLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.FogInspector &&
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
