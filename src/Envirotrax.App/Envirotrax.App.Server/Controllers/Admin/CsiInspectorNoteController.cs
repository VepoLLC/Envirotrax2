using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/csi/inspectors/{professionalId}/notes")]
public class CsiInspectorNoteController : ProfessionalNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.CsiInspector;

    public CsiInspectorNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
