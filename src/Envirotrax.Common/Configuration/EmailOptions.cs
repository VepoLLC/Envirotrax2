
namespace Envirotrax.Common.Configuration;

public class EmailOptions
{

    public string ApiKey { get; set; } = null!;

    public string NoreplyAddress { get; set; } = "noreply@mail.envirotrax.com";
    public string TeamAddress { get; set; } = "team@mail.envirotrax.com";
    public string InfoAddress { get; set; } = "info@mail.envirotrax.com";

    public string? OverrideRecipients { get; set; }
}