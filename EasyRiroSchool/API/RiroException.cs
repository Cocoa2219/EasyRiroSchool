namespace EasyRiroSchool.API;

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