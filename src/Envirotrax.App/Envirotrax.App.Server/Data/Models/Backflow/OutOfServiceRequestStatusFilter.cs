namespace Envirotrax.App.Server.Data.Models.Backflow;

public enum OutOfServiceRequestStatusFilter
{
    AllUncleared = 0,       // OutOfServiceDate == null && ClearedDate == null
    MarkedOutOfService = 1, // OutOfServiceDate != null
    Cleared = 2,            // ClearedDate != null
    All = 3
}
