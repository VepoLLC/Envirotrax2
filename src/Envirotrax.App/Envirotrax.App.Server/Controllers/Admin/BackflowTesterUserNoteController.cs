using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/backflow/testers/{professionalId}/users/{userId}/notes")]
public class BackflowTesterUserNoteController : ProfessionalUserNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.Bpat;

    public BackflowTesterUserNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
