using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi;

[Route("api/csi/inspectors/{professionalId}/users")]
[PermissionResource(PermissionType.CsiInspectors)]
public class CsiInspectorPaymentBypassController : ProfessionalPaymentBypassController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.CsiInspector;

    public CsiInspectorPaymentBypassController(IProfessionalUserService userService)
        : base(userService)
    {
    }
}
