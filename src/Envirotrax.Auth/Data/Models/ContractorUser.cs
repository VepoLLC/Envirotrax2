
using System.Diagnostics.Contracts;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.Auth.Data.Models;

[ExcludedModel]
[ReadOnlyModel]
public class ContractorUser
{
    [AppPrimaryKey(false)]
    public int ContractorId { get; set; }
    public Contractor? Contractor { get; set; }

    [AppPrimaryKey(false)]
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}