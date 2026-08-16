using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi;

public class CsiInspectionRepository : Repository<CsiInspection>, ICsiInspectionRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public CsiInspectionRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    protected override IQueryable<CsiInspection> GetListQuery()
    {
        return base.GetListQuery()
            .Include(c => c.Site)
            .Include(c => c.WaterSupplier);
    }

    protected override IQueryable<CsiInspection> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(c => c.Site)
            .Include(c => c.WaterSupplier)
                .ThenInclude(w => w!.State)
            .Include(c => c.Professional)
            .Include(c => c.Inspector)
                .ThenInclude(i => i!.User)
            .Include(c => c.PropertyState)
            .Include(c => c.MailingState);
    }

    public async Task<IEnumerable<CsiInspection>> SearchForProfessionalAsync(
        PageInfo pageInfo,
        Query query,
        bool latestOnly,
        CancellationToken cancellationToken)
    {
        var dbQuery = GetListQuery()
            .Where(c => c.Site != null && !c.Site.OutOfArea)
            .Where(query.Filter);

        dbQuery = await ApplyLatestOnlyFilterAsync(dbQuery, latestOnly, cancellationToken);

        var paginated = await dbQuery
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CsiInspection>> SearchForAdminAsync(
        PageInfo pageInfo,
        Query query,
        CsiPaymentStatus? paymentStatus,
        CancellationToken cancellationToken)
    {
        var dbQuery = GetListQuery()
            .Include(c => c.PropertyState)
            .Where(query.Filter);

        if (paymentStatus == CsiPaymentStatus.Paid)
        {
            dbQuery = dbQuery.Where(c => c.TransactionId != null && c.TransactionId != string.Empty);
        }

        if (paymentStatus == CsiPaymentStatus.Unpaid)
        {
            dbQuery = dbQuery.Where(c => c.TransactionId == null || c.TransactionId == string.Empty);
        }

        var paginated = await dbQuery
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CsiInspection>> SearchForWaterSupplierAsync(
        PageInfo pageInfo,
        Query query,
        int? subAccountWaterSupplierId,
        CancellationToken cancellationToken)
    {
        var dbQuery = GetListQuery();

        // This is for the dashboard "View" button on a sub account. Normally every query only sees
        // the logged-in water supplier's own inspections, so we turn that off here - but only after
        // checking that the id passed in really is one of this water supplier's sub accounts. If it's
        // not, we just search the water supplier's own inspections like normal.
        if (subAccountWaterSupplierId.HasValue)
        {
            var isOwnChild = await DbContext.WaterSuppliers
                .AnyAsync(ws => ws.Id == subAccountWaterSupplierId.Value && ws.ParentId == _tenantProvider.WaterSupplierId, cancellationToken);

            if (isOwnChild)
            {
                dbQuery = dbQuery
                    .IgnoreQueryFilters()
                    .Where(c => c.WaterSupplierId == subAccountWaterSupplierId.Value);
            }
        }

        var paginated = await dbQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<CsiInspection?> UpdateForAdminAsync(int id, CsiInspectionAdminUpdateRequest request)
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

        inspection.ReasonForInspection = request.ReasonForInspection;
        inspection.InspectionDate = request.InspectionDate;

        inspection.Compliance1 = request.Compliance1;
        inspection.Compliance2 = request.Compliance2;
        inspection.Compliance3 = request.Compliance3;
        inspection.Compliance4 = request.Compliance4;
        inspection.Compliance5 = request.Compliance5;
        inspection.Compliance6 = request.Compliance6;

        inspection.MaterialServiceLineLead = request.MaterialServiceLineLead;
        inspection.MaterialServiceLineCopper = request.MaterialServiceLineCopper;
        inspection.MaterialServiceLinePVC = request.MaterialServiceLinePVC;
        inspection.MaterialServiceLineOther = request.MaterialServiceLineOther;
        inspection.MaterialServiceLineOtherDescription = request.MaterialServiceLineOther ? request.MaterialServiceLineOtherDescription : null;

        inspection.MaterialSolderLead = request.MaterialSolderLead;
        inspection.MaterialSolderLeadFree = request.MaterialSolderLeadFree;
        inspection.MaterialSolderSolventWeld = request.MaterialSolderSolventWeld;
        inspection.MaterialSolderOther = request.MaterialSolderOther;
        inspection.MaterialSolderOtherDescription = request.MaterialSolderOther ? request.MaterialSolderOtherDescription : null;

        inspection.AiOssf = request.AiOssf;
        inspection.AiWaterWell = request.AiWaterWell;
        inspection.AiFireSystem = request.AiFireSystem;
        inspection.AiFireSystem2 = request.AiFireSystem2;
        inspection.AiGreaseTrap = request.AiGreaseTrap;
        inspection.AiSandGrit = request.AiSandGrit;
        inspection.AiReclaimedWater = request.AiReclaimedWater;
        inspection.AiIrrigationSystem = request.AiIrrigationSystem;
        inspection.AiIrrigationSystem2 = request.AiIrrigationSystem2;

        inspection.Comments = request.Comments;

        await DbContext.SaveChangesAsync();

        return inspection;
    }

    public async Task<CsiInspection?> UpdateApprovalAsync(int id, CsiInspectionApprovalRequest request, CancellationToken cancellationToken)
    {
        var inspection = await GetAsync(id, cancellationToken);
        if (inspection == null) return null;

        inspection.Disapproved = request.Disapproved;
        inspection.DisapprovedReason = request.Disapproved ? request.DisapprovedReason : null;

        DbContext.Entry(inspection).Property(x => x.Disapproved).IsModified = true;
        DbContext.Entry(inspection).Property(x => x.DisapprovedReason).IsModified = true;

        await DbContext.SaveChangesAsync(cancellationToken);

        return inspection;
    }

    private static async Task<IQueryable<CsiInspection>> ApplyLatestOnlyFilterAsync(IQueryable<CsiInspection> query, bool latestOnly, CancellationToken cancellationToken)
    {
        if (!latestOnly)
        {
            return query;
        }

        var latestIds = await query
            .GroupBy(c => c.SiteId)
            .Select(g => g.OrderByDescending(c => c.InspectionDate).ThenByDescending(c => c.Id).First().Id)
            .ToListAsync(cancellationToken);

        return query.Where(c => latestIds.Contains(c.Id));
    }
}
