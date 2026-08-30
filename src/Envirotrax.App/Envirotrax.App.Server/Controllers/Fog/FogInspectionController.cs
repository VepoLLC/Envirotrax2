using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/inspections")]
[HasFeature(FeatureType.FogInspection)]
[PermissionResource(PermissionType.FogInspections)]
public class FogInspectionController : WaterSupplierCrudController<FogInspectionDto>
{
    private readonly IFogInspectionService _inspectionService;

    public FogInspectionController(IFogInspectionService service)
        : base(service)
    {
        _inspectionService = service;
    }

    protected override Task<IPagedData<FogInspectionDto>> ProcessGetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var subAccountWaterSupplierId = ReadSubAccountWaterSupplierId();

        if (subAccountWaterSupplierId.HasValue)
        {
            return _inspectionService.SearchForSubAccountAsync(pageInfo, query, subAccountWaterSupplierId.Value, cancellationToken);
        }

        return base.ProcessGetAllAsync(pageInfo, query, cancellationToken);
    }

    private int? ReadSubAccountWaterSupplierId()
    {
        if (int.TryParse(Request.Query["subAccountWaterSupplierId"], out var waterSupplierId))
        {
            return waterSupplierId;
        }

        return null;
    }
}
