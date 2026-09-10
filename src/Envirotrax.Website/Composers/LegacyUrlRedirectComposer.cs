
using Envirotrax.Website.Configuration;
using Umbraco.Cms.Core.Composing;

namespace Envirotrax.Website.Composers;

public class LegacyUrlRedirectComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.Configure<LegacyUrlRedirectOptions>(builder.Config.GetSection("LegacyUrlRedirects"));
    }
}
