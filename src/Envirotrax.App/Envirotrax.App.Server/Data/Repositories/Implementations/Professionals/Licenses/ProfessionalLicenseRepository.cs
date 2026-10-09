using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals.Licenses;

public class ProfessionalLicenseRepository : Repository<ProfessionalLicense>, IProfessionalLicenseRepository
{
    public ProfessionalLicenseRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<ProfessionalLicense> GetListQuery()
    {
        return base.GetListQuery()
            .Include(license => license.LicenseType);
    }

    protected override IQueryable<ProfessionalLicense> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(license => license.LicenseType);
    }

    public async Task<IEnumerable<ProfessionalLicense>> GetAllByProfessionalAsync(int professionalId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await GetListQuery()
            .Where(license => license.ProfessionalId == professionalId && license.ProfessionalType == professionalType)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalLicense>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, ProfessionalType professionalType, CancellationToken cancellationToken)
    {
        return await GetListQuery()
            .Where(license => professionalIds.Contains(license.ProfessionalId) && license.ProfessionalType == professionalType)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProfessionalLicense> UpdateForWaterSupplierAsync(int id, string licenseNumber, DateTime? expirationDate)
    {
        var license = await DbContext.ProfessionalLicenses
            .Include(license => license.LicenseType)
            .Include(license => license.Professional)
            .Include(license => license.CreatedBy)
            .FirstOrDefaultAsync(license => license.Id == id
                && DbContext.ProfessionalWaterSuppliers.Any(registration => registration.ProfessionalId == license.ProfessionalId))
            ?? throw new InvalidOperationException($"License {id} not found for current water supplier.");

        license.LicenseNumber = licenseNumber;
        license.ExpirationDate = expirationDate;

        await SaveChangesAsync(logData: true);

        return license;
    }

    public async Task<ProfessionalLicense> DeleteForWaterSupplierAsync(int id)
    {
        var license = await DbContext.ProfessionalLicenses
            .FirstOrDefaultAsync(license => license.Id == id
                && DbContext.ProfessionalWaterSuppliers.Any(registration => registration.ProfessionalId == license.ProfessionalId))
            ?? throw new InvalidOperationException($"License {id} not found for current water supplier.");

        DbContext.ProfessionalLicenses.Remove(license);
        await DbContext.SaveChangesAsync();

        return license;
    }
}
