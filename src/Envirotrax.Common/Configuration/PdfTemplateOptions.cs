namespace Envirotrax.Common.Configuration;

public class PdfTemplateOptions
{
    public bool IsDevelopment { get; set; }
    public string? ChromiumExecutablePath { get; set; }
    public string BaseUrl { get; set; } = null!;
}
