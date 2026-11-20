using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/transporters/{professionalId}/users/{userId}/notes")]
public class FogTransporterUserNoteController : ProfessionalUserNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogTransporter;

    public FogTransporterUserNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
