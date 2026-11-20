using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/csi/inspectors/{professionalId}/users/{userId}/notes")]
public class CsiInspectorUserNoteController : ProfessionalUserNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.CsiInspector;

    public CsiInspectorUserNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
