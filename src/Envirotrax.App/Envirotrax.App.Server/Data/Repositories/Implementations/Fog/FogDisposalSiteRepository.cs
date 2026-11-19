using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogDisposalSiteRepository : Repository<FogDisposalSite>, IFogDisposalSiteRepository
{
    public FogDisposalSiteRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<FogDisposalSite?> UpdateSiteAsync(FogDisposalSite model)
    {
        var site = await GetTrackedForUpdateAsync(model.Id, default);

        if (site == null)
        {
            return null;
        }

        site.Name = model.Name;
        site.Address = model.Address;
        site.City = model.City;
        site.StateId = model.StateId;
        site.ZipCode = model.ZipCode;
        site.PhoneNumber = model.PhoneNumber;
        site.EmailAddress = model.EmailAddress;
        site.County = model.County;
        site.TceqRegion = model.TceqRegion;
        site.RegistrationNumber = model.RegistrationNumber;
        site.PermitNumber = model.PermitNumber;
        site.PhysicalType = model.PhysicalType;
        site.LocationDescription = model.LocationDescription;
        site.Latitude = model.Latitude;
        site.Longitude = model.Longitude;

        await SaveChangesAsync(logData: true);

        return site;
    }
}
