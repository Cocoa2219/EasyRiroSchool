namespace EasyRiroSchool.Models.Deserialization;

/// <summary>
/// Attribute to mark a property as a Riro table item with a specific index.
/// </summary>
/// <param name="index">The index of the table item.</param>
[AttributeUsage(AttributeTargets.Property)]
public class RiroTableItemAttribute(int index) : Attribute
{
    public int Index { get; } = index;
    public string? GetValueFunc { get; }

    public RiroTableItemAttribute(int index, string? getValueFunc) : this(index)
    {
        GetValueFunc = getValueFunc;
    }
}