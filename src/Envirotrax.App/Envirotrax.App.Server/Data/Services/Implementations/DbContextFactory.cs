
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Services.Implementations;

public class DbContextFactory : IDbContextFactory
{
    private readonly ITenantProvidersService _tenantProvider;
    private readonly TenantDbContext _tenantDbContext;
    private readonly ContractorDbContext _contractorDbContext;

    public DbContextFactory(
        ITenantProvidersService tenantProvider,
        TenantDbContext tenantDbContext,
        ContractorDbContext contractorDbContext)
    {
        _tenantProvider = tenantProvider;
        _tenantDbContext = tenantDbContext;
        _contractorDbContext = contractorDbContext;
    }

    public TenantDbContext CreateContext()
    {
        if (_tenantProvider.ContractorId > 0 || _tenantProvider.ParentContractorId > 0)
        {
            return _contractorDbContext;
        }

        return _tenantDbContext;
    }
}