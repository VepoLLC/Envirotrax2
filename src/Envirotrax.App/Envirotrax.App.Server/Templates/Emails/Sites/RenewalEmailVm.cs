namespace Envirotrax.App.Server.Templates.Emails.Sites;

public class RenewalEmailVm
{
    public int VerificationId { get; set; }

    public string Token { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;
}
