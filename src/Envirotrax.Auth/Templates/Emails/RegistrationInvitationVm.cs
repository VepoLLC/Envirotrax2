
namespace Envirotrax.Auth.Templates.Emails;

public class RegistrationInvitationVm
{
    public string CallbackUrl { get; set; } = null!;
    public string InvitedByCompany { get; set; } = null!;
}