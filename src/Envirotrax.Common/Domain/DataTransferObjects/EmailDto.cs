
namespace Envirotrax.Common.Domain.DataTransferObjects;

public class EmailDto : EmailDto<object>
{

}

public class EmailDto<TTemplate>
{
    public FromAddressType FromAddress { get; set; } = FromAddressType.NorReply;
    public IEnumerable<string> Recipients { get; set; } = null!;
    public string? Subject { get; set; }
    public string? TemplateId { get; set; }
    public TTemplate? TemplateData { get; set; }
    public IEnumerable<EmailAttachmentDto> Attachments { get; set; } = [];
}

public class EmailAttachmentDto
{
    public string Name { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
}

public enum FromAddressType
{
    NorReply,
    Team,
    Info
}