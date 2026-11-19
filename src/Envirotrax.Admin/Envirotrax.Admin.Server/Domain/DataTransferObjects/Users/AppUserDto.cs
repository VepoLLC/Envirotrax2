namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Users;

/// <summary>
/// Mirrors the App's AppUserDto.
/// </summary>
public class AppUserDto
{
    public int Id { get; set; }

    public string? Email { get; set; }
}
