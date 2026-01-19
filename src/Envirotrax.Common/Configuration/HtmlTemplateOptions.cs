
using System.Reflection;

namespace Envirotrax.Common.Configuration;

public class HtmlTemplateOptions
{
    public Assembly Assembly { get; set; } = null!;
    public string Namespace { get; set; } = null!;
}