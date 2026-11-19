using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/transporters/{professionalId}/users/{userId}/notes")]
[HasFeature(FeatureType.FogTransportation)]
[PermissionResource(PermissionType.FogTransporters)]
public class FogTransporterUserNotesController : ProfessionalUserNotesController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogTransporter;

    public FogTransporterUserNotesController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
