namespace Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

public class LetterHistoryDto : LetterProcessDto
{
    public List<LetterFileDto> Files { get; set; } = [];
}
