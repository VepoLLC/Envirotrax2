namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Letters;

public class GeneratedLettersDto
{
    public LetterCountsDto Counts { get; set; } = new();

    public int EmailCount { get; set; }

    public string? FileName { get; set; }
    public byte[]? FileContent { get; set; }
}

public class LetterCountsDto
{
    public int ExpiringNotice1 { get; set; }
    public int ExpiringNotice2 { get; set; }
    public int ExpiredNotice1 { get; set; }
    public int ExpiredNotice2 { get; set; }
    public int BackflowNonCompliant1 { get; set; }
    public int BackflowNonCompliant2 { get; set; }
    public int UnknownAssembliesNotice { get; set; }

    public int ImpendingNotice1 { get; set; }
    public int ImpendingNotice2 { get; set; }
    public int PastDueNotice1 { get; set; }
    public int PastDueNotice2 { get; set; }
    public int CsiNonCompliant1 { get; set; }
    public int CsiNonCompliant2 { get; set; }
}
