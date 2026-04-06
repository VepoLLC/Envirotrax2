using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi
{
    public class CsiInspectorRepository : Repository<Professional>, ICsiInspectorRepository
    {
        public CsiInspectorRepository(IDbContextSelector dbContextSelector)
            : base(dbContextSelector)
        {
        }

        public async Task<Professional?> GetWithStateAsync(int id, CancellationToken cancellationToken)
        {
            return await DbContext.Professionals
                .AsNoTracking()
                .Include(p => p.State)
                .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<List<ProfessionalWaterSupplier>> GetWaterSuppliersAsync(int professionalId, CancellationToken cancellationToken)
        {
            return await DbContext.ProfessionalWaterSuppliers
                .AsNoTracking()
                .Include(pws => pws.WaterSupplier)
                .Where(pws => pws.ProfessionalId == professionalId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ProfessionalUser>> GetSubAccountsAsync(int professionalId, CancellationToken cancellationToken)
        {
            return await DbContext.ProfessionalUsers
                .AsNoTracking()
                .Include(pu => pu.User)
                .Where(pu => pu.ProfessionalId == professionalId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ProfessionalUserLicense>> GetLicensesAsync(int professionalId, CancellationToken cancellationToken)
        {
            return await DbContext.ProfessionalUserLicenses
                .AsNoTracking()
                .Include(l => l.LicenseType)
                .Include(l => l.User)
                .Where(l => l.ProfessionalId == professionalId)
                .ToListAsync(cancellationToken);
        }
    }
}
