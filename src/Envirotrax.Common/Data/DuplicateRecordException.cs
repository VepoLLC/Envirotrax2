
namespace Envirotrax.Common.Data
{
    public class DuplicateRecordException : Exception
    {
        /// <summary>
        /// Names of the entity properties backing the unique index or constraint that was violated,
        /// resolved from EF Core model metadata. Empty when the violated index/constraint could not be matched.
        /// </summary>
        public IReadOnlyList<string> PropertyNames { get; }

        public DuplicateRecordException(Exception innerException, IReadOnlyList<string>? propertyNames = null)
            : base("The record must be unique. A value in this record is duplicated. Please check all fields and try again.", innerException)
        {
            PropertyNames = propertyNames ?? Array.Empty<string>();
        }

        public DuplicateRecordException(string message, Exception innerException, IReadOnlyList<string>? propertyNames = null)
            : base(message, innerException)
        {
            PropertyNames = propertyNames ?? Array.Empty<string>();
        }
    }
}