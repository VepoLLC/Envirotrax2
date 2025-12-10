
namespace Envirotrax.Common.Data
{
    public class DuplicateRecordException : Exception
    {
        public DuplicateRecordException(Exception innerException)
            : base("The record must be unique. A value in this record is duplicated. Please check all fields and try again.", innerException)
        {

        }

        public DuplicateRecordException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}