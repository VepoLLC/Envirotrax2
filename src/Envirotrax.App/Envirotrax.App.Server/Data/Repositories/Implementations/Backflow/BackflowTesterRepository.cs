using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
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

        public async Task<IEnumerable<Professional>> SearchAsync(BackflowTesterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
        {
            var dbQuery = GetListQuery()
                .Where(query.Filter);

            if (!string.IsNullOrWhiteSpace(criteria.BpatLicenseNumber))
            {
                var bpatLicenseNumber = criteria.BpatLicenseNumber;

                dbQuery = dbQuery.Where(p => DbContext.ProfessionalUserLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.Bpat &&
                    !l.LicenseType!.IsFireLicense &&
                    l.LicenseNumber.Contains(bpatLicenseNumber)));
            }

            if (!string.IsNullOrWhiteSpace(criteria.FireLicenseNumber))
            {
                var fireLicenseNumber = criteria.FireLicenseNumber;

                dbQuery = dbQuery.Where(p => DbContext.ProfessionalUserLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.Bpat &&
                    l.LicenseType!.IsFireLicense &&
                    l.LicenseNumber.Contains(fireLicenseNumber)));
            }

            if (!string.IsNullOrWhiteSpace(criteria.InsurancePolicyNumber))
            {
                var insurancePolicyNumber = criteria.InsurancePolicyNumber;

                dbQuery = dbQuery.Where(p => DbContext.ProfessionalInsurances.Any(i =>
                    i.ProfessionalId == p.Id &&
                    i.InsuranceNumber.Contains(insurancePolicyNumber)));
            }

            if (!string.IsNullOrWhiteSpace(criteria.UserEmail))
            {
                var userEmail = criteria.UserEmail;

                dbQuery = dbQuery.Where(p =>
                    p.CompanyEmail!.Contains(userEmail) ||
                    DbContext.ProfessionalUsers.Any(u =>
                        u.ProfessionalId == p.Id &&
                        u.IsBackflowTester &&
                        u.User!.Email!.Contains(userEmail)));
            }

            if (!string.IsNullOrWhiteSpace(criteria.ContactName))
            {
                var contactName = criteria.ContactName;

                dbQuery = dbQuery.Where(p => DbContext.ProfessionalUsers.Any(u =>
                    u.ProfessionalId == p.Id &&
                    u.IsBackflowTester &&
                    u.ContactName!.Contains(contactName)));
            }

            if (!string.IsNullOrWhiteSpace(criteria.CellNumber))
            {
                var cellNumber = criteria.CellNumber;

                dbQuery = dbQuery.Where(p => DbContext.ProfessionalUsers.Any(u =>
                    u.ProfessionalId == p.Id &&
                    u.IsBackflowTester &&
                    u.User!.PhoneNumber!.Contains(cellNumber)));
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
