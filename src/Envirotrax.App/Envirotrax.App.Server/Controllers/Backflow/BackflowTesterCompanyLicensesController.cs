using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

[Route("api/backflow/testers")]
[HasFeature(FeatureType.BackflowTesting)]
[PermissionResource(PermissionType.BackflowTesters)]
public class BackflowTesterCompanyLicensesController : WaterSupplierProtectedController
{
    private readonly IProfessionalLicenseService _licenseService;

    public BackflowTesterCompanyLicensesController(IProfessionalLicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    [HttpGet("{id}/company-licenses")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetLicensesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _licenseService.GetAllByProfessionalAsync(id, ProfessionalType.Bpat, pageInfo, query, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id}/company-licenses")]
    [HasFeature(FeatureType.ManageProfessionalLicenses)]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> AddLicenseAsync(int id, [FromBody] ProfessionalLicenseDto dto)
    {
        var result = await _licenseService.AddForProfessionalAsync(id, dto);

        return Ok(result);
    }

    [HttpPut("{id}/company-licenses/{licenseId}")]
    [HasFeature(FeatureType.ManageProfessionalLicenses)]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateLicenseAsync(int id, int licenseId, [FromBody] ProfessionalLicenseDto dto)
    {
        dto.Id = licenseId;

        var result = await _licenseService.UpdateForProfessionalAsync(id, dto);

        return Ok(result);
    }

    [HttpDelete("{id}/company-licenses/{licenseId}")]
    [HasFeature(FeatureType.ManageProfessionalLicenses)]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> DeleteLicenseAsync(int licenseId)
    {
        await _licenseService.DeleteAsync(licenseId);

        return Ok();
    }
}
