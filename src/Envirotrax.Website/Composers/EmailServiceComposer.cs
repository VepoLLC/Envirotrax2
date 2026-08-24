
using System.Reflection;
using Envirotrax.Common.Configuration;
using Umbraco.Cms.Core.Composing;

namespace Envirotrax.Website.Composers;

public class EmailServiceComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddEmailService(builder.Config.GetSection("Email"), options =>
        {
            options.Assembly = Assembly.GetExecutingAssembly();
            options.Namespace = "Envirotrax.Website";
        });
    }
}
