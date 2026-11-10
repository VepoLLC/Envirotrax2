using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

[Route("api/backflow/testers/{professionalId}/notes")]
[HasFeature(FeatureType.BackflowTesting)]
[PermissionResource(PermissionType.BackflowTesters)]
public class BackflowTesterNotesController : ProfessionalNotesController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.Bpat;

    public BackflowTesterNotesController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
