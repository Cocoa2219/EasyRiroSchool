using HtmlAgilityPack;

namespace EasyRiroSchool.Models.Deserialization.Items;

/// <summary>
/// Represents a meal application item in the Riro system.
/// </summary>
[RiroItem("meal")]
public class MealApplicationItem : RiroItem
{
    [RiroTableItem(0)]
    public int Id { get; internal set; }
    public object GetId(HtmlNode node) =>
        int.TryParse(node.InnerText.Trim(), out var id) ? id : -1;

    [RiroTableItem(1)]
    public string? Title { get; internal set; }
    public object GetTitle(HtmlNode node) =>
        node.SelectSingleNode("./a")?.InnerText.Trim() ?? "Title not found";

    [RiroTableItem(2)]
    public bool HasAttachment { get; internal set; }
    public object GetHasAttachment(HtmlNode node) =>
        node.SelectSingleNode("./div")?.InnerText.Trim() != "-";

    [RiroTableItem(3)]
    public string? Author { get; internal set; }
    public object GetAuthor(HtmlNode node) =>
        node.InnerText.Trim();

    [RiroTableItem(4)]
    public int Views { get; internal set; }
    public object GetViews(HtmlNode node) =>
        int.TryParse(node.InnerText.Trim(), out var views) ? views : 0;

    [RiroTableItem(5)]
    public DateTime CreatedAt { get; internal set; }
    public object GetCreatedAt(HtmlNode node) =>
        DateTime.TryParse(node.InnerText.Trim(), out var date) ? date : DateTime.MinValue;

    public override string ToString()
    {
        return $"MealApplicationItem(Id: {Id}, Title: {Title}, Author: {Author}, CreatedAt: {CreatedAt})";
    }
}