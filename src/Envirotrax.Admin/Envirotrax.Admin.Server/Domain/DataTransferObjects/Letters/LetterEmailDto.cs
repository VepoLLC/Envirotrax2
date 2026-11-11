namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Letters;

public class LetterEmailDto
{
    public string FileName { get; set; } = null!;

    public string AccountNumber { get; set; } = null!;

    public string? StreetNumber { get; set; }

    public string? StreetName { get; set; }
}
