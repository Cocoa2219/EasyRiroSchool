namespace EasyRiroSchool.Models.Deserialization.Items;

public class CalendarItem : RiroItem
{
    internal CalendarItem(DateOnly date, IReadOnlyList<string> events, bool isHoliday = false)
    {
        Date = date;
        Events = events ?? throw new ArgumentNullException(nameof(events), "Events cannot be null.");

        IsHoliday = Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday || isHoliday;
    }

    public DateOnly Date { get; }
    public IReadOnlyList<string> Events { get; }
    public DayOfWeek DayOfWeek => Date.DayOfWeek;
    public bool IsHoliday { get; }
}