namespace Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

public class LetterMessageLineDto
{
    public List<LetterMessageRunDto> Runs { get; set; } = [];
}

public class LetterMessageRunDto
{
    public string Text { get; set; } = null!;
    public bool IsBold { get; set; }
    public bool IsItalic { get; set; }
    public bool IsUnderline { get; set; }
}
