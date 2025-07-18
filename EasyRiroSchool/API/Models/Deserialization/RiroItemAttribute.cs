namespace EasyRiroSchool.API.Models.Deserialization;

/// <summary>
/// Attribute to mark a class as a Riro item with a specific path.
/// </summary>
/// <param name="path">The path associated with the Riro item.</param>
[AttributeUsage(AttributeTargets.Class)]
public class RiroItemAttribute(string path) : Attribute
{
    public string Path { get; } = path;
}