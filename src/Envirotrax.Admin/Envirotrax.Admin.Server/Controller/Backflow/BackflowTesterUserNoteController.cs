using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Backflow;

[Route("api/backflow/testers/{professionalId}/users/{userId}/notes")]
public class BackflowTesterUserNoteController : ProfessionalUserNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.Bpat;

    public BackflowTesterUserNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
