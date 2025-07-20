namespace EasyRiroSchool.Models;

/// <summary>
/// Represents information about a database item.
/// </summary>
/// <param name="id">The unique identifier for the database item.</param>
/// <param name="category">The category of the database item, defaulting to 0.</param>
public struct DbInfo(DbId id, int category = 0, int count = 20)
{
    public DbId Id { get; set; } = id;
    public int Category { get; set; } = category;
    public int Count { get; set; } = count;
}