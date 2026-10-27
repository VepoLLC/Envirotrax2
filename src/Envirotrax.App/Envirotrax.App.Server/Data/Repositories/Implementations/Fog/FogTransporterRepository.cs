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

        public async Task<IEnumerable<Professional>> SearchAsync(string? registrationNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken)
        {
            var query = GetListQuery();

            if (!string.IsNullOrWhiteSpace(registrationNumber))
            {
                query = query.Where(p => DbContext.ProfessionalLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.FogTransporter &&
                    l.LicenseNumber.Contains(registrationNumber)));
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

        public async Task<IEnumerable<Professional>> SearchForAdminAsync(FogTransporterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
        {
            var dbQuery = GetListQuery()
                .Where(query.Filter);

            if (!string.IsNullOrWhiteSpace(criteria.LicenseNumber))
            {
                var licenseNumber = criteria.LicenseNumber;

                dbQuery = dbQuery.Where(p => DbContext.ProfessionalLicenses.Any(l =>
                    l.ProfessionalId == p.Id &&
                    l.ProfessionalType == ProfessionalType.FogTransporter &&
                    l.LicenseNumber.Contains(licenseNumber)));
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
                        u.IsFogTransporter &&
                        u.User!.Email!.Contains(userEmail)));
            }

            if (!string.IsNullOrWhiteSpace(criteria.ContactName))
            {
                var contactName = criteria.ContactName;

                dbQuery = dbQuery.Where(p => DbContext.ProfessionalUsers.Any(u =>
                    u.ProfessionalId == p.Id &&
                    u.IsFogTransporter &&
                    u.ContactName!.Contains(contactName)));
            }

            if (!string.IsNullOrWhiteSpace(criteria.CellNumber))
            {
                var cellNumber = criteria.CellNumber;

                dbQuery = dbQuery.Where(p =>
                    p.CellPhoneNumber!.Contains(cellNumber) ||
                    DbContext.ProfessionalUsers.Any(u =>
                        u.ProfessionalId == p.Id &&
                        u.IsFogTransporter &&
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
