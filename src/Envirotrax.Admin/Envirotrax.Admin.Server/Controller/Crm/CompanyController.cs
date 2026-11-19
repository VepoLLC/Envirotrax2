using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Crm;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Crm;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Crm;

[Route("api/crm/companies")]
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
    public async Task<IActionResult> AddAsync([FromBody] CompanyDto company, CancellationToken cancellationToken)
    {
        var added = await _companyService.AddAsync(company, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] CompanyDto company, CancellationToken cancellationToken)
    {
        var updated = await _companyService.UpdateAsync(id, company, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _companyService.DeleteAsync(id, cancellationToken);

        return Ok();
    }
}
