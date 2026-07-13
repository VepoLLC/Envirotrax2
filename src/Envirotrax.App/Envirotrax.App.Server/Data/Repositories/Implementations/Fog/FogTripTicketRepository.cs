using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogTripTicketRepository : Repository<FogTripTicket>, IFogTripTicketRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantProvidersService _tenantProvider;

    public FogTripTicketRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _context = dbContextSelector.Current;
        _tenantProvider = tenantProvider;
    }

    protected override IQueryable<FogTripTicket> GetListQuery()
    {
        return base.GetListQuery()
            .Include(t => t.WaterSupplier)
            .Include(t => t.Site)
            .Include(t => t.Professional);
    }

    protected override IQueryable<FogTripTicket> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
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

        IQueryable<FogTripTicket> dbQuery = _context.FogTripTickets
            .IgnoreQueryFilters()
            .Include(t => t.WaterSupplier)
            .Include(t => t.Site)
            .Include(t => t.Professional);

        if (waterSupplierId.HasValue)
        {
            dbQuery = dbQuery.Where(t => t.WaterSupplierId == waterSupplierId.Value);
        }

        var paginated = await dbQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, ct);

        return await paginated.ToListAsync(ct);
    }
}
