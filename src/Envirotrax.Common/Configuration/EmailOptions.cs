
namespace Envirotrax.Common.Configuration;

public class EmailOptions
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string Host { get; set; } = null!;
    public int Port { get; set; } = 587;

    public string NoreplyAddress { get; set; } = "Envirotrax.com<noreply@envirotrax.com>";
    public string TeamAddress { get; set; } = "Envirotrax.com<team@envirotrax.com>";
    public string InfoAddress { get; set; } = "Envirotrax.com<info@envirotrax.com>";

    public string? OverrideRecipients { get; set; }
}