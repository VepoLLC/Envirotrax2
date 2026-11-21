using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/inspectors/{professionalId}/users/{userId}/notes")]
public class FogInspectorUserNoteController : ProfessionalUserNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogInspector;

    public FogInspectorUserNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
