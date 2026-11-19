namespace Envirotrax.App.Server.Data.Models.Crm;

public enum NextContactDateStatus
{
    None = 0,
    Upcoming = 1,

    // The next contact date is today or has passed.
    Due = 2
}
