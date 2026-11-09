using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/transporters/{professionalId}/users")]
[PermissionResource(PermissionType.FogTransporters)]
public class FogTransporterPaymentBypassController : ProfessionalPaymentBypassController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogTransporter;

    public FogTransporterPaymentBypassController(IProfessionalUserService userService)
        : base(userService)
    {
    }
}
