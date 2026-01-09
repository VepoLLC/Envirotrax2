using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.States;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.States;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.States;

[Route("api/states")]
public class StatesController : CrudController<StateDto>
{
    public StatesController(IStateService service) : base(service)
    {
    }
}
