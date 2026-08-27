using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi;

public class CsiInspectionRepository : Repository<CsiInspection>, ICsiInspectionRepository
{
    public CsiInspectionRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
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

    public async Task<AdminUpdateResult<CsiInspection>> UpdateForAdminAsync(int id, CsiInspectionAdminUpdateRequest request)
    {
        var result = new AdminUpdateResult<CsiInspection>();

        var inspection = await Entity.SingleOrDefaultAsync(i => i.Id == id);

        if (inspection == null)
        {
            return result;
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

        result.Changes = BuildChangeDescription(inspection);

        await DbContext.SaveChangesAsync();

        result.Model = inspection;

        return result;
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
