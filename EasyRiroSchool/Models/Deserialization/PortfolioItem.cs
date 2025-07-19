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
    public int Id { get; internal set; }
    public object GetId(HtmlNode node) =>
        int.Parse(node.InnerText.Trim());

    [RiroTableItem(1, "GetItemType")]
    public PortfolioType Type { get; internal set; }
    public object GetItemType(HtmlNode node) =>
        node.SelectSingleNode("./div")?.InnerText.Trim() switch
        {
            "대기" => PortfolioType.Waiting,
            "제출" => PortfolioType.InProgress,
            "마감" => PortfolioType.Completed,
            "투표" => PortfolioType.Vote,
            _ => PortfolioType.None
        };

    public enum PortfolioType
    {
        None,
        Waiting,
        InProgress,
        Completed,
        Vote
    }

    [RiroTableItem(2)]
    public string? RawTitle { get; internal set; }
    public object GetRawTitle(HtmlNode node) =>
        node.SelectNodes("./a")?[1].InnerText.Trim() ?? "Title not found";

    private PortfolioTitleParts? _titleParts;

    public enum PortfolioSubject
    {
        Autonomy,
        Volunteer,
        ClubActivity,
        Career,
        Read,
        Korean,
        English,
        Math,
        Social,
        Science,
        Entertainment,
        Etc
    }

    private struct PortfolioTitleParts
    {
        public PortfolioSubject? Subject { get; internal set; }
        public Grade Grade { get; internal set; }
        public int Year { get; internal set; }
        public string? Title { get; internal set; }

        public PortfolioTitleParts(string rawTitle)
        {
            var parts = rawTitle.Split('-');
            if (parts.Length < 2)
            {
                throw new RiroApiException("Invalid portfolio title format.");
            }

            Title = parts[1].Trim();
            var subParts = parts[0].Trim().Split(' ');
            if (subParts.Length < 3)
            {
                throw new RiroApiException("Invalid portfolio title format.");
            }
            Year = int.Parse(subParts[0].TrimEnd('년'));
            Grade = subParts[1] switch
            {
                "1학년" => Grade.First,
                "2학년" => Grade.Second,
                "3학년" => Grade.Third,
                "전학년" => Grade.All,
                _ => throw new RiroApiException("Unknown grade in portfolio title.")
            };

            Subject = subParts[2] switch
            {
                "자율" => PortfolioSubject.Autonomy,
                "봉사" => PortfolioSubject.Volunteer,
                "동아리" => PortfolioSubject.ClubActivity,
                "진로" => PortfolioSubject.Career,
                "독서" => PortfolioSubject.Read,
                "국어" => PortfolioSubject.Korean,
                "영어" => PortfolioSubject.English,
                "수학" => PortfolioSubject.Math,
                "사회" => PortfolioSubject.Social,
                "과학" => PortfolioSubject.Science,
                "예체" => PortfolioSubject.Entertainment,
                "기타" => PortfolioSubject.Etc,
                _ => throw new RiroApiException("Unknown subject in portfolio title.")
            };
        }
    }

    public string Title
    {
        get
        {
            if (_titleParts == null)
            {
                if (string.IsNullOrEmpty(RawTitle))
                {
                    throw new RiroApiException("RawTitle is null or empty.");
                }
                _titleParts = new PortfolioTitleParts(RawTitle);
            }
            return _titleParts.Value.Title ?? throw new RiroApiException("Title is not set.");
        }
    }

    public PortfolioSubject Subject
    {
        get
        {
            if (_titleParts == null)
            {
                if (string.IsNullOrEmpty(RawTitle))
                {
                    throw new RiroApiException("RawTitle is null or empty.");
                }
                _titleParts = new PortfolioTitleParts(RawTitle);
            }
            return _titleParts.Value.Subject ?? throw new RiroApiException("Subject is not set.");
        }
    }

    public Grade Grade
    {
        get
        {
            if (_titleParts == null)
            {
                if (string.IsNullOrEmpty(RawTitle))
                {
                    throw new RiroApiException("RawTitle is null or empty.");
                }
                _titleParts = new PortfolioTitleParts(RawTitle);
            }
            return _titleParts.Value.Grade;
        }
    }

    public int Year
    {
        get
        {
            if (_titleParts == null)
            {
                if (string.IsNullOrEmpty(RawTitle))
                {
                    throw new RiroApiException("RawTitle is null or empty.");
                }
                _titleParts = new PortfolioTitleParts(RawTitle);
            }
            return _titleParts.Value.Year;
        }
    }

    [RiroTableItem(3)]
    public bool Submitted { get; internal set; }
    public object GetSubmitted(HtmlNode node) =>
        node.SelectSingleNode("./span") != null;

    [RiroTableItem(4)]
    public int SubmitCount { get; internal set; }
    public object GetSubmitCount(HtmlNode node) =>
        int.Parse(node.SelectSingleNode("./p")?.InnerText.TrimEnd('명') ?? "0");

    [RiroTableItem(5)]
    public string? Teacher { get; internal set; }
    public object GetTeacher(HtmlNode node) =>
        node.InnerText.Trim();

    [RiroTableItem(6)]
    public bool HasAttachments { get; internal set; }
    public object GetHasAttachments(HtmlNode node) =>
        node.SelectSingleNode("./span") != null;

    [RiroTableItem(7)]
    public DateTime StartDate { get; internal set; }

    [RiroTableItem(7)]
    public DateTime EndDate { get; internal set; }

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
        return $"Portfolio(Id: {Id}, Type: {Type}, Title: {RawTitle}, Submitted: {Submitted}, " +
               $"SubmitCount: {SubmitCount}, Teacher: {Teacher}, HasAttachments: {HasAttachments}, " +
               $"StartDate: {StartDate}, EndDate: {EndDate})";
    }
}