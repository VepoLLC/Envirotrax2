using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

namespace Envirotrax.App.Server.Templates.Emails.Letters;

public class LetterEmailVm
{
    public LetterContentDto Content { get; set; } = null!;

    public LetterDto Letter { get; set; } = null!;
}
