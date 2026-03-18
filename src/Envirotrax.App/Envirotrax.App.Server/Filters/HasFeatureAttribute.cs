using Envirotrax.Common;

namespace Envirotrax.App.Server.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class HasFeatureAttribute : Attribute
{
    public FeatureType[] Features { get; }

    public HasFeatureAttribute(params FeatureType[] features)
    {
        Features = features;
    }
}