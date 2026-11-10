using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi;

[Route("api/csi/inspectors/{professionalId}/notes")]
[HasFeature(FeatureType.CsiInspection)]
[PermissionResource(PermissionType.CsiInspectors)]
public class CsiInspectorNotesController : ProfessionalNotesController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.CsiInspector;

    public CsiInspectorNotesController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
