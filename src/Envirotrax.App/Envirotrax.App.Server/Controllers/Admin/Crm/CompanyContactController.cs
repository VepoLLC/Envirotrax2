using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.App.Server.Domain.Services.Definitions.Crm;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin.Crm;

/// <summary>
/// A CRM company's additional contacts (V1 Vepo Manager, the Additional Contacts of a contact window).
/// </summary>
[Route("api/admin/crm/companies/{companyId}/contacts")]
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
        var contacts = await _contactService.GetAllByCompanyAsync(companyId, pageInfo, query, cancellationToken);

        return Ok(contacts);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int companyId, [FromBody] CompanyContactDto contact, CancellationToken cancellationToken)
    {
        var added = await _contactService.AddForCompanyAsync(companyId, contact, cancellationToken);

        return added == null ? NotFound() : Ok(added);
    }

    [HttpPut("{contactId}")]
    public async Task<IActionResult> UpdateAsync(int companyId, int contactId, [FromBody] CompanyContactDto contact, CancellationToken cancellationToken)
    {
        var updated = await _contactService.UpdateForCompanyAsync(companyId, contactId, contact, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{contactId}")]
    public async Task<IActionResult> DeleteAsync(int companyId, int contactId, CancellationToken cancellationToken)
    {
        var deleted = await _contactService.DeleteForCompanyAsync(companyId, contactId, cancellationToken);

        return deleted == null ? NotFound() : Ok();
    }
}
