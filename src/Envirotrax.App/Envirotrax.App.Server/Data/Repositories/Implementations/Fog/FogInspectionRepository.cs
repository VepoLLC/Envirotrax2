using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogInspectionRepository : Repository<FogInspection>, IFogInspectionRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public FogInspectionRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    protected override IQueryable<FogInspection> GetListQuery()
    {
        return base.GetListQuery()
            .Include(fi => fi.Site)
            .Include(fi => fi.WaterSupplier);
    }

    protected override IQueryable<FogInspection> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(fi => fi.Site)
            .Include(fi => fi.WaterSupplier)
            .ThenInclude(ws => ws!.State)
            // ReferencedProfessionalUserDto.EmailAddress maps from User.Email, so without this ThenInclude the
            // inspector's Email Address renders blank on every FOG details view. CsiInspectionRepository already
            // does this for its inspector.
            .Include(fi => fi.Inspector)
            .ThenInclude(inspector => inspector!.User)
            .Include(fi => fi.PropertyState)
            .Include(fi => fi.MailingState);
    }

    public override Task<IEnumerable<FogInspection>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(FogInspection.Id)] = SortOperator.Asc;
        }
        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    public async Task<IEnumerable<FogInspection>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query,
        bool latestOnly, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
            query.Sort[nameof(FogInspection.Id)] = SortOperator.Asc;

        ProfessionalRecordScope.ApplyToProfessionalSearch(query, _tenantProvider.ProfessionalId, nameof(FogInspection.ProfessionalId));

        var filteredQ = GetListQuery()
            .Where(query.Filter);

        if (latestOnly)
        {
            var filteredInspections = filteredQ;
            filteredQ = filteredInspections.Where(fi =>
                !filteredInspections.Any(other =>
                    other.SiteId == fi.SiteId &&
                    (other.InspectionDate > fi.InspectionDate ||
                        (other.InspectionDate == fi.InspectionDate && other.Id > fi.Id))));
        }

        var paginated = await filteredQ
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    // Ownership is enforced by ProfessionalDbContext (FogInspection is an ISharedProfessionalModel); a
    // non-owner's save throws rather than returning null here. Only the not-found/already-paid guard is ours.
    public async Task<FogInspection?> UpdateForProfessionalAsync(
        FogInspection model,
        string? newExteriorImagePath,
        string? newInteriorImagePath,
        string? newSignatureImagePath)
    {
        var inspection = await GetTrackedForUpdateAsync(model.Id, default);

        if (inspection == null || !string.IsNullOrEmpty(inspection.TransactionId))
        {
            return null;
        }

        inspection.InspectionDate = model.InspectionDate;
        inspection.FacilityType = model.FacilityType;
        inspection.ReasonForInspection = model.ReasonForInspection;

        inspection.InterceptorType = model.InterceptorType;
        inspection.InterceptorOtherDescription = model.InterceptorOtherDescription;
        inspection.InterceptorCapacity = model.InterceptorCapacity;
        inspection.InterceptorCapacityType = model.InterceptorCapacityType;
        inspection.InterceptorLocationDescription = model.InterceptorLocationDescription;
        inspection.InterceptorLatitude = model.InterceptorLatitude;
        inspection.InterceptorLongitude = model.InterceptorLongitude;
        inspection.InterceptorComments = model.InterceptorComments;

        inspection.Maintained = model.Maintained;
        inspection.Accessible = model.Accessible;
        inspection.PastOverflow = model.PastOverflow;

        inspection.InletChamberWettingHeight = model.InletChamberWettingHeight;
        inspection.InletChamberGreaseBlanket = model.InletChamberGreaseBlanket;
        inspection.InletChamberSediments = model.InletChamberSediments;
        inspection.OutletChamberWettingHeight = model.OutletChamberWettingHeight;
        inspection.OutletChamberGreaseBlanket = model.OutletChamberGreaseBlanket;
        inspection.OutletChamberSediments = model.OutletChamberSediments;
        inspection.InletTeeIntact = model.InletTeeIntact;
        inspection.OutletTeeIntact = model.OutletTeeIntact;
        inspection.InletTeeVisible = model.InletTeeVisible;
        inspection.OutletTeeVisible = model.OutletTeeVisible;

        inspection.SampledFrom = model.SampledFrom;
        inspection.SamplingPointAccessible = model.SamplingPointAccessible;
        inspection.SamplingPointClean = model.SamplingPointClean;

        inspection.InletTotalCapacityPercent = model.InletTotalCapacityPercent;
        inspection.OutletTotalCapacityPercent = model.OutletTotalCapacityPercent;
        inspection.TotalCapacityPercent = model.TotalCapacityPercent;

        inspection.InspectionResult = model.InspectionResult;

        inspection.SignatureContactName = model.SignatureContactName;

        inspection.Comments = model.Comments;

        inspection.FogGeneratorPhoneNumber = model.FogGeneratorPhoneNumber;
        inspection.FogGeneratorEmailAddress = model.FogGeneratorEmailAddress;

        inspection.NeedsValidation = model.NeedsValidation;
        inspection.ValidationSiteInformationChanged = model.ValidationSiteInformationChanged;

        // Site/Inspector snapshot fields — refreshed the same way SubmitAsync populates them for a new row.
        inspection.PropertyBusinessName = model.PropertyBusinessName;
        inspection.PropertyType = model.PropertyType;
        inspection.PropertyStreetNumber = model.PropertyStreetNumber;
        inspection.PropertyStreetName = model.PropertyStreetName;
        inspection.PropertyNumber = model.PropertyNumber;
        inspection.PropertyCity = model.PropertyCity;
        inspection.PropertyStateId = model.PropertyStateId;
        inspection.PropertyZip = model.PropertyZip;
        inspection.MailingCompanyName = model.MailingCompanyName;
        inspection.MailingContactName = model.MailingContactName;
        inspection.MailingStreetNumber = model.MailingStreetNumber;
        inspection.MailingStreetName = model.MailingStreetName;
        inspection.MailingNumber = model.MailingNumber;
        inspection.MailingCity = model.MailingCity;
        inspection.MailingStateId = model.MailingStateId;
        inspection.MailingZip = model.MailingZip;
        inspection.MailingPhoneNumber = model.MailingPhoneNumber;
        inspection.MailingEmailAddress = model.MailingEmailAddress;

        inspection.InspectorId = model.InspectorId;
        inspection.InspectorCompanyName = model.InspectorCompanyName;
        inspection.InspectorContactName = model.InspectorContactName;
        inspection.InspectorJobTitle = model.InspectorJobTitle;
        inspection.InspectorAddress = model.InspectorAddress;
        inspection.InspectorCity = model.InspectorCity;
        inspection.InspectorState = model.InspectorState;
        inspection.InspectorZip = model.InspectorZip;
        inspection.InspectorWorkNumber = model.InspectorWorkNumber;
        inspection.InspectorCellNumber = model.InspectorCellNumber;
        inspection.InspectorFaxNumber = model.InspectorFaxNumber;

        if (newExteriorImagePath != null)
        {
            inspection.ExteriorImagePath = newExteriorImagePath;
        }
        if (newInteriorImagePath != null)
        {
            inspection.InteriorImagePath = newInteriorImagePath;
        }
        if (newSignatureImagePath != null)
        {
            inspection.SignatureImagePath = newSignatureImagePath;
            inspection.SignatureDate = DateTime.UtcNow;
        }
        else
        {
            inspection.SignatureDate = model.SignatureDate;
        }

        await SaveChangesAsync(logData: true);

        return inspection;
    }

    public async Task<FogInspection?> UpdateForAdminAsync(int id, FogInspectionAdminUpdateRequest request)
    {
        var inspection = await Entity.SingleOrDefaultAsync(i => i.Id == id);

        if (inspection == null)
        {
            return null;
        }

        inspection.PropertyType = request.PropertyType;
        inspection.PropertyBusinessName = request.PropertyBusinessName;
        inspection.PropertyStreetNumber = request.PropertyStreetNumber;
        inspection.PropertyStreetName = request.PropertyStreetName;
        inspection.PropertyNumber = request.PropertyNumber;
        inspection.PropertyCity = request.PropertyCity;
        inspection.PropertyStateId = request.PropertyState?.Id;
        inspection.PropertyZip = request.PropertyZip;

        inspection.MailingCompanyName = request.MailingCompanyName;
        inspection.MailingContactName = request.MailingContactName;
        inspection.MailingStreetNumber = request.MailingStreetNumber;
        inspection.MailingStreetName = request.MailingStreetName;
        inspection.MailingNumber = request.MailingNumber;
        inspection.MailingCity = request.MailingCity;
        inspection.MailingStateId = request.MailingState?.Id;
        inspection.MailingZip = request.MailingZip;

        inspection.InterceptorType = request.InterceptorType;
        inspection.InterceptorOtherDescription = request.InterceptorOtherDescription;
        inspection.InterceptorCapacity = request.InterceptorCapacity;
        inspection.InterceptorCapacityType = request.InterceptorCapacityType;
        inspection.InterceptorLocationDescription = request.InterceptorLocationDescription;

        inspection.InspectionDate = request.InspectionDate;
        inspection.ReasonForInspection = request.ReasonForInspection;
        inspection.FacilityType = request.FacilityType;
        inspection.Maintained = request.Maintained;
        inspection.Accessible = request.Accessible;
        inspection.PastOverflow = request.PastOverflow;
        inspection.SamplingPointAccessible = request.SamplingPointAccessible;
        inspection.SamplingPointClean = request.SamplingPointClean;
        inspection.SampledFrom = request.SampledFrom;
        inspection.InletTeeIntact = request.InletTeeIntact;
        inspection.OutletTeeIntact = request.OutletTeeIntact;
        inspection.InletChamberWettingHeight = request.InletChamberWettingHeight;
        inspection.InletChamberGreaseBlanket = request.InletChamberGreaseBlanket;
        inspection.InletChamberSediments = request.InletChamberSediments;
        inspection.OutletChamberWettingHeight = request.OutletChamberWettingHeight;
        inspection.OutletChamberGreaseBlanket = request.OutletChamberGreaseBlanket;
        inspection.OutletChamberSediments = request.OutletChamberSediments;
        inspection.InletTotalCapacityPercent = request.InletTotalCapacityPercent;
        inspection.OutletTotalCapacityPercent = request.OutletTotalCapacityPercent;
        inspection.TotalCapacityPercent = request.TotalCapacityPercent;
        inspection.InspectionResult = request.InspectionResult;
        inspection.Comments = request.Comments;

        await SaveChangesAsync(logData: true);

        return inspection;
    }

    public async Task<FogInspection?> UpdateImagePathAsync(int id, string imagePathPropertyName, string newPath)
    {
        var inspection = await Entity.SingleOrDefaultAsync(i => i.Id == id);

        if (inspection == null)
        {
            return null;
        }

        DbContext.Entry(inspection).Property(imagePathPropertyName).CurrentValue = newPath;

        await SaveChangesAsync(logData: false);

        return inspection;
    }

    public Task<int> CountBySiteAsync(int siteId, CancellationToken cancellationToken)
    {
        return Entity.CountAsync(f => f.SiteId == siteId && f.DeletedTime == null, cancellationToken);
    }

    public async Task<List<FogInspection>> GetUnpaidForCheckoutAsync(IReadOnlyCollection<int> ids, int professionalId, int? inspectorId, CancellationToken cancellationToken)
    {
        return await GetUnpaidForCheckoutQuery(ids, professionalId, inspectorId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> MarkPaidAsync(
        IReadOnlyCollection<int> ids,
        int professionalId,
        int? inspectorId,
        string transactionId,
        DateTime transactionDate,
        CancellationToken cancellationToken)
    {
        return await GetUnpaidForCheckoutQuery(ids, professionalId, inspectorId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(f => f.TransactionId, transactionId)
                .SetProperty(f => f.TransactionDate, transactionDate), cancellationToken);
    }

    public async Task<decimal> SumAmountByTransactionIdAsync(string transactionId, int professionalId, CancellationToken cancellationToken)
    {
        return await DbContext.FogInspections
            .Where(f => f.TransactionId == transactionId && f.ProfessionalId == professionalId)
            .SumAsync(f => f.Amount, cancellationToken);
    }

    public async Task<List<FogInspection>> GetByTransactionIdAsync(string transactionId, int professionalId, CancellationToken cancellationToken)
    {
        return await GetDetailsQuery()
            .Where(f => f.TransactionId == transactionId && f.ProfessionalId == professionalId)
            .OrderBy(f => f.CreatedTime)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<FogInspection> GetUnpaidForCheckoutQuery(IReadOnlyCollection<int> ids, int professionalId, int? inspectorId)
    {
        return DbContext.FogInspections
            .Where(f => ids.Contains(f.Id)
                && f.ProfessionalId == professionalId
                && (inspectorId == null || f.InspectorId == inspectorId)
                && (f.TransactionId == null || f.TransactionId == string.Empty)
                && f.DeletedTime == null);
    }
}
