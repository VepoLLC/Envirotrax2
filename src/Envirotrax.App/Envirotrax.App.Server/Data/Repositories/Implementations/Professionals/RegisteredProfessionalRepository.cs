
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

/// <summary>
/// Backs the public "Registered Professionals" directory (V1: registrations.aspx).
///
/// Every query here calls <c>IgnoreQueryFilters()</c> on purpose. The directory is anonymous and
/// cross-tenant: an anonymous visitor carries no water supplier, and a signed-in professional would
/// otherwise be scoped to their own company by <c>ProfessionalDbContext</c>. Scoping comes from the
/// explicit <c>waterSupplierId</c> every method takes instead of from the ambient tenant.
/// </summary>
public class RegisteredProfessionalRepository : Repository<Professional>, IRegisteredProfessionalRepository
{
    /// <summary>Accounts registered within this many months are listed first, as they were in V1.</summary>
    private const int NewAccountMonths = 6;

    public RegisteredProfessionalRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<Professional> GetListQuery()
    {
        return base.GetListQuery().IgnoreQueryFilters();
    }

    public async Task<IEnumerable<RegisteredProfessionalSupplier>> GetWaterSuppliersAsync(
        ProfessionalType professionalType,
        CancellationToken cancellationToken)
    {
        var settings = GetProgramSettingsQuery(professionalType);

        return await DbContext.WaterSuppliers
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(supplier => supplier.IsActive
                && supplier.DeletedTime == null
                && settings.Any(programSettings => programSettings.WaterSupplierId == supplier.Id))
            .OrderBy(supplier => supplier.Name)
            .Select(supplier => new RegisteredProfessionalSupplier
            {
                Id = supplier.Id,
                Name = supplier.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RegisteredProfessional>> SearchAsync(
        int waterSupplierId,
        ProfessionalType professionalType,
        PageInfo pageInfo,
        Query query,
        CancellationToken cancellationToken)
    {
        var requirements = await GetSupplierRequirementsAsync(waterSupplierId, professionalType, cancellationToken);

        if (requirements == null)
        {
            return [];
        }

        var now = DateTime.UtcNow;
        var registrations = GetRegistrationsQuery(waterSupplierId, professionalType);

        var professionals = GetListQuery()
            .Where(professional => professional.DeletedTime == null
                && professional.ParentId == null
                && !professional.HidePublicListing
                && registrations.Any(registration => registration.ProfessionalId == professional.Id));

        professionals = ApplyLicenseRequirement(professionals, professionalType, requirements, now);
        professionals = ApplyInsuranceRequirement(professionals, requirements, now);

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(RegisteredProfessional.IsNewAccount)] = SortOperator.Desc;
            query.Sort[nameof(RegisteredProfessional.CompanyName)] = SortOperator.Asc;
        }

        var paginated = await Project(professionals, now)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Reads everything about the water supplier that decides who may be listed: the state whose
    /// licenses are required, and whether an insurance policy is required for this professional type.
    /// Returns null when the supplier cannot publish this directory at all — unknown, inactive,
    /// administrative-only, or simply not running the program.
    /// </summary>
    private async Task<SupplierRequirements?> GetSupplierRequirementsAsync(
        int waterSupplierId,
        ProfessionalType professionalType,
        CancellationToken cancellationToken)
    {
        var supplier = await DbContext.WaterSuppliers
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(candidate => candidate.Id == waterSupplierId
                && candidate.IsActive
                && candidate.DeletedTime == null)
            .Select(candidate => new { candidate.StateId })
            .SingleOrDefaultAsync(cancellationToken);

        if (supplier == null)
        {
            return null;
        }

        var settings = await GetProgramSettingsQuery(professionalType)
            .SingleOrDefaultAsync(programSettings => programSettings.WaterSupplierId == waterSupplierId, cancellationToken);

        if (settings == null)
        {
            return null;
        }

        var hasRequiredLicenseTypes = await DbContext.ProfessionalLicenseTypes
            .AsNoTracking()
            .IgnoreQueryFilters()
            .AnyAsync(licenseType => licenseType.StateId == supplier.StateId
                && licenseType.ProfessionalType == professionalType
                && !licenseType.IsFireLicense, cancellationToken);

        return new SupplierRequirements
        {
            StateId = supplier.StateId,
            HasRequiredLicenseTypes = hasRequiredLicenseTypes,
            RequiresInsurance = RequiresInsurance(settings, professionalType)
        };
    }

    /// <summary>
    /// Settings of every water supplier that publicly runs the program this professional type serves.
    /// </summary>
    private IQueryable<GeneralSettings> GetProgramSettingsQuery(ProfessionalType professionalType)
    {
        var settings = DbContext.GeneralSettings
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(programSettings => !programSettings.AdministrativeOnly);

        return professionalType switch
        {
            ProfessionalType.Bpat => settings.Where(programSettings => programSettings.BackflowTesting),
            ProfessionalType.CsiInspector => settings.Where(programSettings => programSettings.CsiInspections),
            ProfessionalType.FogInspector => settings.Where(programSettings => programSettings.FogProgram),
            ProfessionalType.FogTransporter => settings.Where(programSettings => programSettings.FogProgram),
            _ => settings.Where(programSettings => false)
        };
    }

    /// <summary>
    /// Active, non-banned registrations of the given water supplier for the given professional type.
    /// </summary>
    private IQueryable<ProfessionalWaterSupplier> GetRegistrationsQuery(int waterSupplierId, ProfessionalType professionalType)
    {
        var registrations = DbContext.ProfessionalWaterSuppliers
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(registration => registration.WaterSupplierId == waterSupplierId && !registration.IsBanned);

        return professionalType switch
        {
            ProfessionalType.Bpat => registrations.Where(registration => registration.HasBackflowTesting),
            ProfessionalType.CsiInspector => registrations.Where(registration => registration.HasCsiInpection),
            ProfessionalType.FogInspector => registrations.Where(registration => registration.HasFogInspection),
            ProfessionalType.FogTransporter => registrations.Where(registration => registration.HasFogTransportation),
            _ => registrations.Where(registration => false)
        };
    }

    private IQueryable<ProfessionalUserLicense> GetUnexpiredLicensesQuery(ProfessionalType professionalType, DateTime now)
    {
        return DbContext.ProfessionalUserLicenses
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(license => license.ProfessionalType == professionalType
                && license.ExpirationDate != null
                && license.ExpirationDate > now);
    }

    /// <summary>
    /// Keeps only companies that can actually work for this water supplier: someone in the company
    /// holds an unexpired license of a type the supplier's state requires. A company whose own
    /// licenses fall short still qualifies when one of its sub accounts is licensed, which is how V1
    /// kept multi-branch companies listed. Suppliers in a state with no required license type for
    /// this professional type gate on nothing.
    /// </summary>
    private IQueryable<Professional> ApplyLicenseRequirement(
        IQueryable<Professional> professionals,
        ProfessionalType professionalType,
        SupplierRequirements requirements,
        DateTime now)
    {
        if (!requirements.HasRequiredLicenseTypes)
        {
            return professionals;
        }

        var licenses = GetUnexpiredLicensesQuery(professionalType, now)
            .Where(license => license.LicenseType!.StateId == requirements.StateId
                && !license.LicenseType.IsFireLicense);

        var subAccounts = GetListQuery().Where(subAccount => subAccount.DeletedTime == null);

        return professionals.Where(professional =>
            licenses.Any(license => license.ProfessionalId == professional.Id)
            || subAccounts.Any(subAccount => subAccount.ParentId == professional.Id
                && licenses.Any(license => license.ProfessionalId == subAccount.Id)));
    }

    /// <summary>
    /// Drops companies without an unexpired insurance policy when the supplier requires one.
    /// </summary>
    private IQueryable<Professional> ApplyInsuranceRequirement(
        IQueryable<Professional> professionals,
        SupplierRequirements requirements,
        DateTime now)
    {
        if (!requirements.RequiresInsurance)
        {
            return professionals;
        }

        var insurances = DbContext.ProfessionalInsurances
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(insurance => insurance.ExpirationDate != null && insurance.ExpirationDate > now);

        return professionals.Where(professional =>
            insurances.Any(insurance => insurance.ProfessionalId == professional.Id));
    }

    private IQueryable<RegisteredProfessional> Project(IQueryable<Professional> professionals, DateTime now)
    {
        var newAccountCutoff = now.AddMonths(-NewAccountMonths);

        var contacts = DbContext.ProfessionalUsers
            .AsNoTracking()
            .IgnoreQueryFilters();

        // V1 flagged fireline testers from any unexpired fire license on the account, regardless of
        // which state issued it, so this deliberately does not filter by the supplier's state.
        var fireLicenses = GetUnexpiredLicensesQuery(ProfessionalType.Bpat, now)
            .Where(license => license.LicenseType!.IsFireLicense);

        return professionals
            .Select(professional => new
            {
                Professional = professional,
                Contact = contacts
                    .Where(contact => contact.ProfessionalId == professional.Id && contact.IsAdmin)
                    .OrderBy(contact => contact.UserId)
                    .Select(contact => new
                    {
                        contact.ContactName,
                        EmailAddress = contact.User!.Email,
                        PhoneNumber = contact.User!.PhoneNumber
                    })
                    .FirstOrDefault(),
                HasFireLicense = fireLicenses.Any(license => license.ProfessionalId == professional.Id)
            })
            .Select(row => new RegisteredProfessional
            {
                Id = row.Professional.Id,
                CompanyName = row.Professional.Name,
                ContactName = row.Contact != null ? row.Contact.ContactName : null,
                RegisteredDate = row.Professional.CreatedTime,
                Address = row.Professional.Address,
                City = row.Professional.City,
                State = row.Professional.State != null ? row.Professional.State.Name : null,
                ZipCode = row.Professional.ZipCode,
                WorkNumber = row.Professional.PhoneNumber,
                CellNumber = row.Contact != null ? row.Contact.PhoneNumber : null,
                FaxNumber = row.Professional.FaxNumber,
                EmailAddress = row.Professional.CompanyEmail ?? (row.Contact != null ? row.Contact.EmailAddress : null),
                WebsiteUrl = row.Professional.WebSiteUrl,
                HasFireLicense = row.HasFireLicense,
                IsNewAccount = row.Professional.CreatedTime >= newAccountCutoff
            });
    }

    private static bool RequiresInsurance(GeneralSettings settings, ProfessionalType professionalType)
    {
        return professionalType switch
        {
            ProfessionalType.Bpat => settings.BpatsRequireInsurance,
            ProfessionalType.CsiInspector => settings.CsiInspectorsRequireInsurance,
            ProfessionalType.FogTransporter => settings.FogTransportersRequireInsurance,
            _ => false
        };
    }

    private class SupplierRequirements
    {
        public int? StateId { get; init; }

        public bool HasRequiredLicenseTypes { get; init; }

        public bool RequiresInsurance { get; init; }
    }
}
