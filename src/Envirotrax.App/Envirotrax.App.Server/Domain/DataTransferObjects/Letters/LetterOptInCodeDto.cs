namespace Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

public class LetterOptInCodeDto
{
    public int SiteId { get; set; }
    public string CodeHash { get; set; } = null!;
}
