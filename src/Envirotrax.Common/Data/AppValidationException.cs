
namespace Envirotrax.Common.Data;

public class AppValidationException : Exception
{
    public AppValidationException(string message)
        : base(message)
    {
    }

    public AppValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
