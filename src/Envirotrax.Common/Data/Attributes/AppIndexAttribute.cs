
namespace Envirotrax.Common.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AppIndexAttribute : Attribute
    {
        public IEnumerable<string> PropertyNames { get; }

        public string? Name { get; set; }
        public bool IsUnique { get; set; }

        public AppIndexAttribute(params string[] propertyNames)
        {
            PropertyNames = propertyNames;
        }
    }
}