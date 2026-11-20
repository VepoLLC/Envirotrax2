using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Csi;

[Route("api/csi/inspectors/{professionalId}/users/{userId}/notes")]
public class CsiInspectorUserNoteController : ProfessionalUserNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.CsiInspector;

    public CsiInspectorUserNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
