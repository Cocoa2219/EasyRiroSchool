namespace EasyRiroSchool.API.Exceptions;

/// <summary>
/// Exception thrown when there is an issue with Riro API operations.
/// </summary>
public class RiroApiException : RiroException
{
    public RiroApiException(string message) : base(message)
    {
    }

    public RiroApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}