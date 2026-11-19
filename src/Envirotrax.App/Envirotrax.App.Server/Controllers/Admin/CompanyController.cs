using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.App.Server.Domain.Services.Definitions.Crm;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

/// <summary>
/// The CRM companies of Envirotrax.Admin (V1 Vepo Manager, Contact Management). Deleted companies are soft-deleted
/// and dropped from the list by the global QueryFilter.
/// </summary>
[Route("api/admin/crm/companies")]
public class CompanyController : AdminBaseController
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var companies = await _companyService.GetAllAsync(pageInfo, query, cancellationToken);

        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var company = await _companyService.GetAsync(id, cancellationToken);

        return company == null ? NotFound() : Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] CompanyDto company)
    {
        var added = await _companyService.AddAsync(company);

        return Ok(added);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] CompanyDto company, CancellationToken cancellationToken)
    {
        if (!await _companyService.ExistsAsync(id, cancellationToken))
        {
            return NotFound();
        }

        company.Id = id;

        var updated = await _companyService.UpdateAsync(company);

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deleted = await _companyService.DeleteAsync(id);

        return deleted == null ? NotFound() : Ok();
    }
}
