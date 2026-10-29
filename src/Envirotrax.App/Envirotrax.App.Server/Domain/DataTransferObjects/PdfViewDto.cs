namespace Envirotrax.App.Server.Domain.DataTransferObjects;

public class PdfViewDto
{
    public string Token { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public byte[] Content { get; set; } = null!;
}

public class PdfViewLinkDto
{
    public string Url { get; set; } = null!;
}
