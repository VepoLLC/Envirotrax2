namespace Envirotrax.App.Server.Data.Models.Crm;

// The values are V1's ContactManagerContacts.Type, so the legacy migration copies the column as it is.
public enum ContactType
{
    ExistingClient = 0,
    ProspectiveClient = 1,
    PastClient = 2,
    CompetitorClient = 3,
    Technology = 4
}
