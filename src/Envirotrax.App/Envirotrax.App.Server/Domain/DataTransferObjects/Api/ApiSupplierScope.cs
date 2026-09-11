namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

public class ApiSupplierScope
{
    private ApiSupplierScope(ApiSupplierScopeKind kind, int waterSupplierId, IReadOnlyCollection<int> allowedWaterSupplierIds)
    {
        Kind = kind;
        WaterSupplierId = waterSupplierId;
        AllowedWaterSupplierIds = allowedWaterSupplierIds;
    }

    public ApiSupplierScopeKind Kind { get; }

    /// <summary>
    /// The supplier the account is bound to. Zero for an open account.
    /// </summary>
    public int WaterSupplierId { get; }

    /// <summary>
    /// For a master account, its child suppliers. V1 does not include the master's own supplier
    /// here, so a master cannot narrow a request to its own records.
    /// </summary>
    public IReadOnlyCollection<int> AllowedWaterSupplierIds { get; }

    public static ApiSupplierScope Open()
    {
        return new ApiSupplierScope(ApiSupplierScopeKind.Open, 0, Array.Empty<int>());
    }

    public static ApiSupplierScope Master(int waterSupplierId, IReadOnlyCollection<int> childWaterSupplierIds)
    {
        return new ApiSupplierScope(ApiSupplierScopeKind.Master, waterSupplierId, childWaterSupplierIds);
    }

    public static ApiSupplierScope Single(int waterSupplierId)
    {
        return new ApiSupplierScope(ApiSupplierScopeKind.Single, waterSupplierId, new[] { waterSupplierId });
    }
}
