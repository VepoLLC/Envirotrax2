using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogTripTicketRepository : Repository<FogTripTicket>, IFogTripTicketRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public FogTripTicketRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    protected override IQueryable<FogTripTicket> GetListQuery()
    {
        return base.GetListQuery()
            .Include(t => t.WaterSupplier)
            .Include(t => t.Site)
            .Include(t => t.Professional);
    }

    // Transporter is an optional navigation to a ProfessionalUser, which the professional context filters to
    // the logged-in professional's own users; left unfiltered that would silently null out another
    // professional's Transporter on a shared ticket instead of hiding the row (TransporterId is nullable, so
    // EF left-joins it). Lifting only that named filter keeps the other contexts' filters, same as
    // CsiInspectionRepository.GetDetailsQuery.
    protected override IQueryable<FogTripTicket> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .IgnoreQueryFilters([ProfessionalDbContext.OwnProfessionalFilterFor<ProfessionalUser>()])
            .Include(t => t.WaterSupplier)
            .ThenInclude(ws => ws!.State)
            .Include(t => t.Site)
            .Include(t => t.Professional)
            .Include(t => t.Transporter)
            .Include(t => t.Vehicle)
            .Include(t => t.ReceiverDisposalSite)
            .Include(t => t.PropertyState)
            .Include(t => t.CreatedBy)
            .Include(t => t.UpdatedBy);
    }

    public override Task<IEnumerable<FogTripTicket>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(FogTripTicket.Id)] = SortOperator.Asc;
        }

        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    public async Task<IEnumerable<FogTripTicket>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, int? waterSupplierId, CancellationToken ct)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(FogTripTicket.Id)] = SortOperator.Asc;
        }

        var dbQuery = GetListQuery();

        if (waterSupplierId.HasValue)
        {
            dbQuery = dbQuery.Where(t => t.WaterSupplierId == waterSupplierId.Value);
        }
        else
        {
            ProfessionalRecordScope.ApplyToProfessionalSearch(query, _tenantProvider.ProfessionalId, nameof(FogTripTicket.ProfessionalId));
        }

        var paginated = await dbQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, ct);

        return await paginated.ToListAsync(ct);
    }

    public async Task<FogTripTicket?> UpdateApprovalAsync(int id, bool disapproved, int? approvedById, CancellationToken cancellationToken)
    {
        var ticket = await GetNoIncludesAsync(id, cancellationToken);

        if (ticket == null)
        {
            return null;
        }

        DbContext.Attach(ticket);
        ticket.Disapproved = disapproved;

        if (!disapproved)
        {
            ticket.ApprovalDate = DateTime.UtcNow;
            ticket.ApprovedById = approvedById;
        }

        await DbContext.SaveChangesAsync();

        return ticket;
    }

    public Task<int> CountBySiteAsync(int siteId, CancellationToken cancellationToken)
    {
        return Entity.CountAsync(t => t.SiteId == siteId && t.DeletedTime == null, cancellationToken);
    }

    public async Task<List<FogTripTicket>> GetUnpaidForCheckoutAsync(IReadOnlyCollection<int> ids, int professionalId, int? transporterId, CancellationToken cancellationToken)
    {
        return await GetUnpaidForCheckoutQuery(ids, professionalId, transporterId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> MarkPaidAsync(
        IReadOnlyCollection<int> ids,
        int professionalId,
        int? transporterId,
        string transactionId,
        DateTime transactionDate,
        IReadOnlyCollection<int> emailPdfTicketIds,
        CancellationToken cancellationToken)
    {
        return await GetUnpaidForCheckoutQuery(ids, professionalId, transporterId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(t => t.TransactionId, transactionId)
                .SetProperty(t => t.TransactionDate, transactionDate)
                .SetProperty(t => t.EmailPdf, t => emailPdfTicketIds.Contains(t.Id)), cancellationToken);
    }

    public async Task<decimal> SumAmountByTransactionIdAsync(string transactionId, int professionalId, CancellationToken cancellationToken)
    {
        return await DbContext.FogTripTickets
            .Where(t => t.TransactionId == transactionId && t.ProfessionalId == professionalId)
            .SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<List<FogTripTicket>> GetByTransactionIdAsync(string transactionId, int professionalId, CancellationToken cancellationToken)
    {
        return await GetDetailsQuery()
            .Where(t => t.TransactionId == transactionId && t.ProfessionalId == professionalId)
            .OrderBy(t => t.CreatedTime)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<FogTripTicket> GetUnpaidForCheckoutQuery(IReadOnlyCollection<int> ids, int professionalId, int? transporterId)
    {
        return DbContext.FogTripTickets
            .Where(t => ids.Contains(t.Id)
                && t.ProfessionalId == professionalId
                && (transporterId == null || t.TransporterId == transporterId)
                && (t.TransactionId == null || t.TransactionId == string.Empty)
                && t.Completed
                && t.DeletedTime == null);
    }
}
