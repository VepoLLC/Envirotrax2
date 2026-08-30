using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow
{
    public class BackflowTesterRepository : Repository<Professional>, IBackflowTesterRepository
    {
        public BackflowTesterRepository(IDbContextSelector dbContextSelector)
            : base(dbContextSelector)
        {
        }

        protected override IQueryable<Professional> GetDetailsQuery()
        {
            return base.GetDetailsQuery()
                .Include(p => p.State);
        }

        protected override IQueryable<Professional> GetListQuery()
        {
            return base.GetListQuery()
                .Include(p => p.State)
                .Where(p => p.HasBackflowTesting);
        }

        public async Task<IEnumerable<Professional>> SearchAsync(string? bpatLicenseNumber, string? fireLicenseNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken)
        {
            var query = GetListQuery();

            if (!string.IsNullOrWhiteSpace(bpatLicenseNumber))
            {
                query = query.Where(p => DbContext.ProfessionalUserLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.Bpat &&
                    !l.LicenseType!.IsFireLicense &&
                    l.LicenseNumber.Contains(bpatLicenseNumber)));
            }

            if (!string.IsNullOrWhiteSpace(fireLicenseNumber))
            {
                query = query.Where(p => DbContext.ProfessionalUserLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.Bpat &&
                    l.LicenseType!.IsFireLicense &&
                    l.LicenseNumber.Contains(fireLicenseNumber)));
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
