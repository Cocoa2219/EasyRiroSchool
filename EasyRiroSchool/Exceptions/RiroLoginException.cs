namespace EasyRiroSchool.API.Exceptions;

/// <summary>
/// Exception thrown when there is an issue with Riro login.
/// </summary>
public class RiroLoginException : RiroException
{
    public RiroLoginException(string message) : base(message)
    {
    }

    public RiroLoginException(string message, Exception innerException) : base(message, innerException)
    {
    }
}