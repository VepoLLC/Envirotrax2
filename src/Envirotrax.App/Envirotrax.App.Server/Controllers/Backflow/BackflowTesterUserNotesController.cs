using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

[Route("api/backflow/testers/{professionalId}/users/{userId}/notes")]
[HasFeature(FeatureType.BackflowTesting)]
[PermissionResource(PermissionType.BackflowTesters)]
public class BackflowTesterUserNotesController : ProfessionalUserNotesController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.Bpat;

    public BackflowTesterUserNotesController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
