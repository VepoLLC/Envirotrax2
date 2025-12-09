
namespace Envirotrax.Common.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class AppPrimaryKeyAttribute : Attribute
    {
        /// <summary>
        /// If you specify more than one key, this will determine the composite key order. e.g. (Column1, Column2). Good for many-to-many relationships.
        /// </summary>
        public int CompositeKeyOrder { get; set; }

        /// <summary>
        /// Tells whether the column should be identity. Should be true if this is your only primary key column.
        /// </summary>
        public bool ValueGeneratedOnAdd { get; }

        /// <summary>
        /// Tells whether this primary key should be used in the repository queries.
        /// </summary>
        public bool IsShadowKey { get; set; }

        public AppPrimaryKeyAttribute(bool valueGeneratedOnAdd)
        {
            ValueGeneratedOnAdd = valueGeneratedOnAdd;
        }
    }
}