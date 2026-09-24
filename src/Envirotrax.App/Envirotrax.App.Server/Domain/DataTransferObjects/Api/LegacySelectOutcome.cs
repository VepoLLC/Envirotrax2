namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

public class LegacySelectOutcome
{
    private LegacySelectOutcome(LegacySelectStatus status, string? message, LegacySelectResult? result)
    {
        Status = status;
        Message = message;
        Result = result;
    }

    public LegacySelectStatus Status { get; }

    public string? Message { get; }

    public LegacySelectResult? Result { get; }

    public static LegacySelectOutcome Success(LegacySelectResult result)
    {
        return new LegacySelectOutcome(LegacySelectStatus.Success, null, result);
    }

    public static LegacySelectOutcome Failure(LegacySelectStatus status, string message)
    {
        return new LegacySelectOutcome(status, message, null);
    }
}
