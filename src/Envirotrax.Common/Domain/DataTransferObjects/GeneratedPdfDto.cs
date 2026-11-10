namespace Envirotrax.Common.Domain.DataTransferObjects;

public class GeneratedPdfDto<TScriptResult>
{
    public byte[] Content { get; set; } = null!;

    public TScriptResult ScriptResult { get; set; } = default!;
}
