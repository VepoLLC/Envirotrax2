using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Csi;

[Route("api/csi/inspectors/{professionalId}/notes")]
public class CsiInspectorNoteController : ProfessionalNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.CsiInspector;

    public CsiInspectorNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
