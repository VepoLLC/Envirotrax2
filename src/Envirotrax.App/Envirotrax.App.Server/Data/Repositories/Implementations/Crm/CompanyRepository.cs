using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Crm;
using Envirotrax.App.Server.Data.Repositories.Definitions.Crm;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Crm;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<Company> GetDetailsQuery()
    {
        return base.GetDetailsQuery().Include(company => company.LastContactedBy);
    }

    public override Task<IEnumerable<Company>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(Company.Name)] = SortOperator.Asc;
        }

        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    // Copies the editable fields onto the stored row, so the row keeps its water supplier (part of the key) and its
    // creation audit whichever context saves it. The admin context does not stamp a tenant on save.
    public async Task<Company?> UpdateCompanyAsync(Company model, CancellationToken cancellationToken)
    {
        var company = await GetTrackedForUpdateAsync(model.Id, cancellationToken);

        if (company == null)
        {
            return null;
        }

        company.ContactType = model.ContactType;
        company.Name = model.Name;
        company.Position = model.Position;
        company.ContactName = model.ContactName;
        company.Address = model.Address;
        company.City = model.City;
        company.StateId = model.StateId;
        company.ZipCode = model.ZipCode;
        company.EmailAddress = model.EmailAddress;
        company.PhoneNumber = model.PhoneNumber;
        company.CellNumber = model.CellNumber;

        company.HasBackflowTesting = model.HasBackflowTesting;
        company.BackflowTestingStartDate = model.BackflowTestingStartDate;
        company.HasCsiInspection = model.HasCsiInspection;
        company.CsiInspectionStartDate = model.CsiInspectionStartDate;
        company.HasFogInspection = model.HasFogInspection;
        company.FogInspectionStartDate = model.FogInspectionStartDate;
        company.HasFogTransportation = model.HasFogTransportation;
        company.FogTransportationStartDate = model.FogTransportationStartDate;

        company.LastContactedById = model.LastContactedById;
        company.LastContactDate = model.LastContactDate;
        company.NextContactDate = model.NextContactDate;

        await SaveChangesAsync();

        return company;
    }
}
