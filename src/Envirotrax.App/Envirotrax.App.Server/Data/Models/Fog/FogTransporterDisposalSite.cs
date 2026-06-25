using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Fog;

[Table("FogTransporterDisposalSites")]
public class FogTransporterDisposalSite : IProfessionalModel, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public int DisposalSiteId { get; set; }
    public FogDisposalSite? DisposalSite { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}
