using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/inspectors/{professionalId}/notes")]
[HasFeature(FeatureType.FogInspection)]
[PermissionResource(PermissionType.FogInspectors)]
public class FogInspectorNotesController : ProfessionalNotesController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogInspector;

    public FogInspectorNotesController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
