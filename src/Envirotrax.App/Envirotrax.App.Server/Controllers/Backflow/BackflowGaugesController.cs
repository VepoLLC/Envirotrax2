
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

[Route("api/professionals/backflow/gauges")]
[Authorize(Roles = $"{RoleDefinitions.Professionals.Admin},{RoleDefinitions.Professionals.BackflowTester}")]
public class BackflowGaugesController : ProfessionalCrudController<BackflowGaugeDto>
{
    private readonly IBackflowGaugeService _gaugeService;

    public BackflowGaugesController(IBackflowGaugeService service)
        : base(service)
    {
        _gaugeService = service;
    }
}
