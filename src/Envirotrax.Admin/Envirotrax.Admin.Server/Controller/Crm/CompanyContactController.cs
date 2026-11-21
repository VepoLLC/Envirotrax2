using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Crm;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Crm;

[Route("api/crm/companies/{companyId}/contacts")]
public class CompanyContactController : AdminBaseController
{
    private readonly ICompanyContactService _contactService;

    public CompanyContactController(ICompanyContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int companyId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var contacts = await _contactService.GetAllAsync(companyId, pageInfo, query, cancellationToken);

        return Ok(contacts);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int companyId, [FromBody] CompanyContactDto contact, CancellationToken cancellationToken)
    {
        var added = await _contactService.AddAsync(companyId, contact, cancellationToken);

        return added == null ? NotFound() : Ok(added);
    }

    [HttpPut("{contactId}")]
    public async Task<IActionResult> UpdateAsync(int companyId, int contactId, [FromBody] CompanyContactDto contact, CancellationToken cancellationToken)
    {
        var updated = await _contactService.UpdateAsync(companyId, contactId, contact, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{contactId}")]
    public async Task<IActionResult> DeleteAsync(int companyId, int contactId, CancellationToken cancellationToken)
    {
        await _contactService.DeleteAsync(companyId, contactId, cancellationToken);

        return Ok();
    }
}
