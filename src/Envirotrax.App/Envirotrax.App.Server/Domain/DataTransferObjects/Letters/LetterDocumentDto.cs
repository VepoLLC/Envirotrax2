namespace Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

public class LetterDocumentDto
{
    public LetterContentDto Content { get; set; } = null!;

    public List<LetterDto> Letters { get; set; } = [];
}
