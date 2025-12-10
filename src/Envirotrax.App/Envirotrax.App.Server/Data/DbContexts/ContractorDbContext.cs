using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.DbContexts
{
    public class ContractorDbContext : TenantDbContext
    {
        public ContractorDbContext(
            DbContextOptions<ContractorDbContext> options,
            ILogger<ContractorDbContext> logger,
            ITenantProvidersService tenantProvider)
            : base(options, logger, tenantProvider)
        {
        }
    }
}