
namespace Envirotrax.Common.Configuration;

public class EmailOptions
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string Host { get; set; } = null!;
    public int Port { get; set; } = 587;

    public string NoreplyAddress { get; set; } = null!;
    public string TeamAddress { get; set; } = null!;
    public string InfoAddress { get; set; } = null!;

    public string? OverrideRecipients { get; set; }
}