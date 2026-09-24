namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

public class ApiSupplierScope
{
    private ApiSupplierScope(
        ApiSupplierScopeKind kind,
        int waterSupplierId,
        int? legacyWaterSupplierId,
        IReadOnlyCollection<int> allowedWaterSupplierIds)
    {
        Kind = kind;
        WaterSupplierId = waterSupplierId;
        LegacyWaterSupplierId = legacyWaterSupplierId;
        AllowedWaterSupplierIds = allowedWaterSupplierIds;
    }

    public ApiSupplierScopeKind Kind { get; }

    /// <summary>
    /// The supplier the account is bound to, as V2's internal id. Zero for an open account. This is
    /// what the query filters compare against; it is never written to the wire.
    /// </summary>
    public int WaterSupplierId { get; }

    /// <summary>
    /// The same supplier's preserved V1 id, used wherever the response reports the restriction back
    /// to the caller. Null when the supplier has no V1 identity.
    /// </summary>
    public int? LegacyWaterSupplierId { get; }

    /// <summary>
    /// For a master account, its child suppliers. V1 does not include the master's own supplier
    /// here, so a master cannot narrow a request to its own records.
    /// </summary>
    public IReadOnlyCollection<int> AllowedWaterSupplierIds { get; }

    public static ApiSupplierScope Open()
    {
        return new ApiSupplierScope(ApiSupplierScopeKind.Open, 0, null, Array.Empty<int>());
    }

    public static ApiSupplierScope Master(
        int waterSupplierId,
        int? legacyWaterSupplierId,
        IReadOnlyCollection<int> childWaterSupplierIds)
    {
        return new ApiSupplierScope(ApiSupplierScopeKind.Master, waterSupplierId, legacyWaterSupplierId, childWaterSupplierIds);
    }

    public static ApiSupplierScope Single(int waterSupplierId, int? legacyWaterSupplierId)
    {
        return new ApiSupplierScope(ApiSupplierScopeKind.Single, waterSupplierId, legacyWaterSupplierId, new[] { waterSupplierId });
    }
}
