
namespace Envirotrax.App.Server.Domain.Configuration;

public class AuthorizeNetOptions
{
    public string BaseUrl { get; set; } = null!;
    public string ApiLoginId { get; set; } = null!;
    public string TransactionKey { get; set; } = null!;
}
