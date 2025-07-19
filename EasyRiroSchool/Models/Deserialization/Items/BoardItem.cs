using HtmlAgilityPack;

namespace EasyRiroSchool.Models.Deserialization.Items;

[RiroItem("board_msg")]
public class BoardItem : RiroItem
{
    [RiroTableItem(0)]
    public int Id { get; internal set; }

    public object GetId(HtmlNode node) =>
        int.TryParse(node.InnerText.Trim(), out var id) ? id : -1;

    [RiroTableItem(1, "GetItemType")]
    public BoardType Type { get; internal set; }
    public object GetItemType(HtmlNode node) =>
        node.SelectSingleNode("./div")?.InnerText.Trim() switch
        {
            "대기" => BoardType.Waiting,
            "제출" => BoardType.InProgress,
            "마감" => BoardType.Completed,
            "알림" => BoardType.Announcement,
            _ => BoardType.None
        };

    public enum BoardType
    {
        None,
        Announcement,
        Waiting,
        InProgress,
        Completed
    }

    [RiroTableItem(2)]
    public BoardTarget Target { get; internal set; }
    public object GetTarget(HtmlNode node) =>
        node.InnerText.Trim() switch
        {
            "전체" => BoardTarget.All,
            "학생" => BoardTarget.Student,
            _ => BoardTarget.All
        };

    public enum BoardTarget
    {
        All,
        Student,
    }

    [RiroTableItem(3)]
    public string? Title { get; internal set; }
    public object GetTitle(HtmlNode node) =>
        node.SelectSingleNode("./a")?.InnerText.Trim() ?? "Title not found";

    [RiroTableItem(4)]
    public bool HasAttachment { get; internal set; }
    public object GetHasAttachment(HtmlNode node) =>
        node.SelectSingleNode("./div")?.InnerText.Trim() != "-";

    [RiroTableItem(5)]
    public string? Author { get; internal set; }
    public object GetAuthor(HtmlNode node) =>
        node.InnerText.Trim() ?? "Author not found";

    [RiroTableItem(6)]
    public int Views { get; internal set; }
    public object GetViews(HtmlNode node) =>
        int.TryParse(node.InnerText.Trim(), out var views) ? views : 0;

    [RiroTableItem(7)]
    public DateTime CreatedAt { get; internal set; }
    public object GetCreatedAt(HtmlNode node) =>
        DateTime.TryParse(node.InnerText.Trim(), out var date) ? date : DateTime.MinValue;

    public override string ToString()
    {
        return $"BoardItem(Id: {Id}, ItemType: {Type}, Target: {Target}, Title: {Title}, HasAttachment: {HasAttachment}, Author: {Author}, Views: {Views}, CreatedAt: {CreatedAt})";
    }
}