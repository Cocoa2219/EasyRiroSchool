using System.Text;
using EasyRiroSchool.API;
using EasyRiroSchool.API.Exceptions;
using HtmlAgilityPack;

namespace EasyRiroSchool.Models.Deserialization;

/// <summary>
/// Represents a portfolio item in the Riro system.
/// </summary>
[RiroItem("portfolio")]
public class PortfolioItem : RiroItem
{
    [RiroTableItem(0)]
    public int Id { get; set; }
    public object GetId(HtmlNode node) =>
        int.Parse(node.InnerText.Trim());

    [RiroTableItem(1)]
    public PortfolioType PortfolioType { get; set; }
    public object GetPortfolioType(HtmlNode node) =>
        node.SelectSingleNode("./div")?.InnerText.Trim() switch
        {
            "대기" => PortfolioType.Waiting,
            "제출" => PortfolioType.InProgress,
            "마감" => PortfolioType.Completed,
            "투표" => PortfolioType.Vote,
            _ => PortfolioType.None
        };

    [RiroTableItem(2)]
    public string? RawTitle { get; set; }
    public object GetTitle(HtmlNode node) =>
        node.SelectNodes("./a")?[1].InnerText.Trim() ?? "Title not found";

    private string? _title;
    private string? _subject;
    private int _grade;
    private int _year;

    public string Title
    {
        get
        {
            if (_title == null)
            {
                var title = RawTitle?.Split('-')[1];

                _title = title?.Trim();
            }
            return _title!;
        }
    }

    [RiroTableItem(3)]
    public bool Submitted { get; set; }
    public object GetSubmitted(HtmlNode node) =>
        node.SelectSingleNode("./span") != null;

    [RiroTableItem(4)]
    public int SubmitCount { get; set; }
    public object GetSubmitCount(HtmlNode node) =>
        int.Parse(node.SelectSingleNode("./p")?.InnerText.TrimEnd('명') ?? "0");

    [RiroTableItem(5)]
    public string? Teacher { get; set; }
    public object GetTeacher(HtmlNode node) =>
        node.InnerText.Trim();

    [RiroTableItem(6)]
    public bool HasAttachments { get; set; }
    public object GetHasAttachments(HtmlNode node) =>
        node.SelectSingleNode("./span") != null;

    [RiroTableItem(7)]
    public DateTime StartDate { get; set; }

    [RiroTableItem(7)]
    public DateTime EndDate { get; set; }

    public object GetStartDate(HtmlNode node) => ParseDates(node)[0];
    public object GetEndDate(HtmlNode node) => ParseDates(node)[1];

    private static DateTime[] ParseDates(HtmlNode node)
    {
        var sb = new StringBuilder();

        foreach (var childNode in node.ChildNodes)
        {
            if (childNode.Name == "br")
            {
                sb.AppendLine();
            }
            else
            {
                sb.Append(childNode.InnerText.Trim());
            }
        }

        var dateStrings = sb.ToString().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (dateStrings.Length != 2)
        {
            throw new RiroApiException("Expected two date strings, but found: " + dateStrings.Length);
        }

        var startDateString = dateStrings[0].Trim();
        var endDateString = dateStrings[1].Trim();

        if (string.IsNullOrEmpty(startDateString) || string.IsNullOrEmpty(endDateString))
        {
            throw new RiroApiException("Start or end date string is empty.");
        }

        if (!DateTime.TryParseExact(startDateString, "MM-dd HH:mm:ss", null,
                System.Globalization.DateTimeStyles.None, out var startDate))
        {
            throw new RiroApiException("Failed to parse start date: " + startDateString);
        }

        if (!DateTime.TryParseExact(endDateString, "MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None,
                out var endDate))
        {
            throw new RiroApiException("Failed to parse end date: " + endDateString);
        }

        return [startDate, endDate];
    }

    public override string ToString()
    {
        return $"Portfolio(Id: {Id}, Type: {PortfolioType}, Title: {RawTitle}, Submitted: {Submitted}, " +
               $"SubmitCount: {SubmitCount}, Teacher: {Teacher}, HasAttachments: {HasAttachments}, " +
               $"StartDate: {StartDate}, EndDate: {EndDate})";
    }
}

public enum PortfolioType
{
    None,
    Waiting,
    InProgress,
    Completed,
    Vote
}