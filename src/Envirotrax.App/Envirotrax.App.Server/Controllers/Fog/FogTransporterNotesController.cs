using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/transporters/{professionalId}/notes")]
[HasFeature(FeatureType.FogTransportation)]
[PermissionResource(PermissionType.FogTransporters)]
public class FogTransporterNotesController : ProfessionalNotesController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogTransporter;

    public FogTransporterNotesController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
