using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/inspectors/{professionalId}/notes")]
public class FogInspectorNoteController : ProfessionalNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogInspector;

    public FogInspectorNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
