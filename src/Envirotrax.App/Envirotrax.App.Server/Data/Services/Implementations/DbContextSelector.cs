
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Services.Implementations;

public class DbContextSelector : IDbContextSelector
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITenantProvidersService _tenantProvider;

    public TenantDbContext Current { get; }

    public DbContextSelector(IServiceProvider serviceProvider, ITenantProvidersService tenantProvider)
    {
        _serviceProvider = serviceProvider;
        _tenantProvider = tenantProvider;

        Current = GetCurrentContext();
    }

    private TenantDbContext GetCurrentContext()
    {
        if (_tenantProvider.ContractorId > 0 || _tenantProvider.ParentContractorId > 0)
        {
            return _serviceProvider.GetRequiredService<ContractorDbContext>();
        }

        return _serviceProvider.GetRequiredService<TenantDbContext>();
    }
}     