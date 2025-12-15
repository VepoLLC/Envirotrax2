
using System.Diagnostics.Contracts;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.App.Server.Data.Models.Contractors;

public class ContractorUser
{
    [AppPrimaryKey(false)]
    public int ContractorId { get; set; }
    public Contractor? Contractor { get; set; }

    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}