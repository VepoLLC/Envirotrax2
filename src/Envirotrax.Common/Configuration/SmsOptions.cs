
namespace Envirotrax.Common.Configuration;

public class SmsOptions
{
    public string Username { get; set; } = null!;
    public string ApiKey { get; set; } = null!;

    public string? OverrideRecipient { get; set; }
}