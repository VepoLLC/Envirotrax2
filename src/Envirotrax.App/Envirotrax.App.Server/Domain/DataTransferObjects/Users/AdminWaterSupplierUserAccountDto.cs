
using Envirotrax.Common;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class AdminWaterSupplierUserAccountDto
{
    public int Id { get; set; }

    public string ContactName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string? CellNumber { get; set; }

    public IEnumerable<UserAccountPermissionDto> Permissions { get; set; } = [];
}

public class UserAccountPermissionDto
{
    public PermissionType Permission { get; set; }

    public bool CanView { get; set; }

    public bool CanModify { get; set; }
}
