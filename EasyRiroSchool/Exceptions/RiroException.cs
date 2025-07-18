namespace EasyRiroSchool.API.Exceptions;

/// <summary>
/// Base class for exceptions related to Riro operations.
/// </summary>
public class RiroException : Exception
{
    public RiroException(string message) : base(message)
    {
    }

    public RiroException(string message, Exception innerException) : base(message, innerException)
    {
    }
}