using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/inspectors/{professionalId}/users")]
[PermissionResource(PermissionType.FogInspectors)]
public class FogInspectorPaymentBypassController : ProfessionalPaymentBypassController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogInspector;

    public FogInspectorPaymentBypassController(IProfessionalUserService userService)
        : base(userService)
    {
    }
}
