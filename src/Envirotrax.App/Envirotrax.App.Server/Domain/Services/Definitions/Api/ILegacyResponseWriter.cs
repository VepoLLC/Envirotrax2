using Envirotrax.App.Server.Domain.DataTransferObjects.Api;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Api;

public interface ILegacyResponseWriter
{
    string WriteError(LegacyApiErrorCode code, string message);

    string WriteRecordset(
        string tableNameAlias,
        ApiSupplierScope scope,
        IReadOnlyList<string> selectFields,
        LegacySelectResult result);
}
