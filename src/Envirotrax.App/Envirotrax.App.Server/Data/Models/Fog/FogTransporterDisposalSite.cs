using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Fog;

public class FogTransporterDisposalSite : IProfessionalModel, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(false, IsShadowKey = true)]
    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    [AppPrimaryKey(false)]
    public int DisposalSiteId { get; set; }
    public FogDisposalSite? DisposalSite { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}
