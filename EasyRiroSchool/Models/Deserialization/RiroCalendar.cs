using System.Collections;
using EasyRiroSchool.Models.Deserialization.Items;
using HtmlAgilityPack;

namespace EasyRiroSchool.Models.Deserialization;

public class RiroCalendar : IReadOnlyCollection<CalendarItem>
{
    public RiroCalendar(HtmlNodeCollection items) : this(items.Skip(1))
    {
    }

    public RiroCalendar(IEnumerable<HtmlNode> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items), "Items collection cannot be null.");
        }

        _items = new List<CalendarItem>();

        foreach (var row in items)
        {
            foreach (var node in row.ChildNodes)
            {
                if (node.NodeType != HtmlNodeType.Element || node.Name != "td")
                {
                    continue;
                }

                if (node.HasClass("not"))
                {
                    continue;
                }

                var date = DateOnly.Parse(node.GetAttributeValue("data-date", "0001-01-01"));

                Console.WriteLine($"InnerText: {node.InnerHtml}");

                var dateDiv = node.SelectSingleNode("./div[contains(concat(' ', normalize-space(@class), ' '), ' day ')]");
                if (dateDiv == null)
                {
                    continue;
                }

                var isHoliday = dateDiv.HasClass("date_holiday");
                var events = node.SelectSingleNode("./div[@class='sel_box']")
                    ?.SelectNodes("./div")?.Select(x => x.SelectSingleNode("./span")?.InnerText.Trim() ?? string.Empty);

                if (events == null)
                {
                    continue;
                }

                var calendarItem = new CalendarItem(date, events.ToList(), isHoliday);
                _items.Add(calendarItem);
            }
        }
    }

    private readonly List<CalendarItem> _items;
    public IEnumerator<CalendarItem> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => _items.Count;

    // Calender indexer (calender[date])
    public CalendarItem this[int index]
    {
        get
        {
            if (index < 1 || index > _items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            return _items[index - 1];
        }
    }
}