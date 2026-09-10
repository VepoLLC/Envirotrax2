
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

/// <summary>
/// Spans two unrelated entities, so it owns none of them and does not derive from <see cref="Repository{TModel}"/>.
/// Both sets carry the <c>IProfessionalModel</c> global query filter, so the union is already scoped to the current professional.
/// </summary>
public class ProfessionalDashboardRepository(IDbContextSelector dbContextSelector) : IProfessionalDashboardRepository
{
    private readonly TenantDbContext _dbContext = dbContextSelector.Current;

    public async Task<IEnumerable<ProfessionalDashboardLicenseInsuranceDto>> GetLicensesAndInsurancesAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var licenseQuery = _dbContext.ProfessionalUserLicenses
            .AsNoTracking()
            .Select(license => new ProfessionalDashboardLicenseInsuranceDto
            {
                Id = license.Id,
                RowType = ProfessionalDashboardRowType.License,
                TypeName = license.LicenseType!.Name,
                Number = license.LicenseNumber,
                AssignedTo = string.IsNullOrEmpty(license.ProfessionalUser!.ContactName)
                    ? license.User!.Email
                    : license.ProfessionalUser!.ContactName,
                ExpirationDate = license.ExpirationDate
            });

        var insuranceQuery = _dbContext.ProfessionalInsurances
            .AsNoTracking()
            .Select(insurance => new ProfessionalDashboardLicenseInsuranceDto
            {
                Id = insurance.Id,
                RowType = ProfessionalDashboardRowType.Insurance,
                TypeName = "Insurance Policy",
                Number = insurance.InsuranceNumber,
                AssignedTo = insurance.Professional!.Name,
                ExpirationDate = insurance.ExpirationDate
            });

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(ProfessionalDashboardLicenseInsuranceDto.ExpirationDate)] = SortOperator.Asc;
        }

        // A UNION has no inherent row order, so Skip/Take repeats or drops rows unless the sort is total.
        // Neither key is a grid column, so appending them can never override a sort the user picked.
        query.Sort[nameof(ProfessionalDashboardLicenseInsuranceDto.RowType)] = SortOperator.Asc;
        query.Sort[nameof(ProfessionalDashboardLicenseInsuranceDto.Id)] = SortOperator.Asc;

        var paginated = await licenseQuery
            .Union(insuranceQuery)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }
}
