
namespace Envirotrax.Common.Configuration
{
    public class InternalApiOptions
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string Scope { get; set; } = null!;
        public string TokenEndpoint { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
    }
}