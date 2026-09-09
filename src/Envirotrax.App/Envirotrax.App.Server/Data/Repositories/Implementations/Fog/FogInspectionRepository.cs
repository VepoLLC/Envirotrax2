using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogInspectionRepository : Repository<FogInspection>, IFogInspectionRepository
{
    public FogInspectionRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
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
            .Include(fi => fi.Inspector)
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

    public async Task<UpdateResult<FogInspection>> UpdateForProfessionalAsync(
        FogInspection model,
        int professionalId,
        string? newExteriorImagePath,
        string? newInteriorImagePath,
        string? newSignatureImagePath)
    {
        var result = new UpdateResult<FogInspection>();

        var inspection = await GetTrackedForUpdateAsync(model.Id, default);

        if (inspection == null || inspection.ProfessionalId != professionalId || !string.IsNullOrEmpty(inspection.TransactionId))
        {
            return result;
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

        inspection.NeedsValidation = true;

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

        result.Changes = BuildChangeDescription(inspection);

        await DbContext.SaveChangesAsync();

        result.Model = inspection;

        return result;
    }
}
