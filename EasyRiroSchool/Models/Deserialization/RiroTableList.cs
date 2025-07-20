using System.Collections;
using System.Reflection;
using EasyRiroSchool.Models.Deserialization;
using EasyRiroSchool.Models.Deserialization.Items;
using HtmlAgilityPack;

namespace EasyRiroSchool.Models.Deserialization;

/// <summary>
/// Represents a list of table items deserialized from an HTML table.
/// </summary>
/// <typeparam name="T">The type of the items in the list, which must inherit from <see cref="RiroItem"/>.</typeparam>
public class RiroTableList<T> : IReadOnlyCollection<T> where T : RiroItem, new()
{
    internal RiroTableList()
    {
        _items = [];
        SetupProperties();
    }

    public RiroTableList(HtmlNodeCollection items) : this(items.Skip(1))
    {
    }

    private static List<RiroTableProperty>? _properties;

    private struct RiroTableProperty
    {
        public PropertyInfo Property { get; set; }
        public RiroTableItemAttribute Attribute { get; set; }
        public MethodInfo GetValueMethod { get; set; }
    }

    public RiroTableList(IEnumerable<HtmlNode> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items), "Items collection cannot be null.");
        }

        var itemsList = new List<T>();
        var htmlNodes = items.ToList();

        SetupProperties();

        for (var i = 1; i < htmlNodes.Count; i++)
        {
            var row = htmlNodes[i];
            var item = new T();

            if (_properties != null)
                foreach (var prop in _properties)
                {
                    var cell = row.ChildNodes
                        .Where(n => n.NodeType == HtmlNodeType.Element && n.Name == "td")
                        .ElementAtOrDefault(prop.Attribute.Index);

                    if (cell == null) continue;

                    var value = prop.GetValueMethod.Invoke(item, [cell]);
                    prop.Property.SetValue(item, value);
                }

            itemsList.Add(item);
        }

        _items = itemsList;
    }

    private void SetupProperties()
    {
        if (_properties != null)
        {
            return;
        }

        foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attribute = prop.GetCustomAttribute<RiroTableItemAttribute>();
            if (attribute != null)
            {
                _properties ??= [];

                if (attribute.GetValueFunc != null)
                {
                    var getValueMethod = typeof(T).GetMethod(attribute.GetValueFunc) ?? typeof(T).GetMethod($"Get{prop.Name}",
                        BindingFlags.Public | BindingFlags.Instance);

                    if (getValueMethod == null)
                    {
                        throw new InvalidOperationException(
                            $"Method '{attribute.GetValueFunc}' or 'Get{prop.Name}' not found in type '{typeof(T).Name}'.");
                    }

                    _properties.Add(new RiroTableProperty
                    {
                        Property = prop,
                        Attribute = attribute,
                        GetValueMethod = getValueMethod
                    });
                }
                else
                {
                    _properties.Add(new RiroTableProperty
                    {
                        Property = prop,
                        Attribute = attribute,
                        GetValueMethod = typeof(T).GetMethod($"Get{prop.Name}",
                            BindingFlags.Public | BindingFlags.Instance) ?? throw new InvalidOperationException(
                            $"Method 'Get{prop.Name}' not found in type '{typeof(T).Name}'.")
                    });
                }
            }
        }
    }

    private readonly List<T> _items;
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal void Append(IEnumerable<HtmlNode> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items), "Items collection cannot be null.");
        }

        var htmlNodes = items.ToList();
        if (htmlNodes.Count == 0) return;

        foreach (var row in htmlNodes)
        {
            var item = new T();

            if (_properties != null)
                foreach (var prop in _properties)
                {
                    var cell = row.ChildNodes
                        .Where(n => n is { NodeType: HtmlNodeType.Element, Name: "td" })
                        .ElementAtOrDefault(prop.Attribute.Index);

                    if (cell == null) continue;

                    var value = prop.GetValueMethod.Invoke(item, [cell]);
                    prop.Property.SetValue(item, value);
                }

            _items.Add(item);
        }
    }

    /// <inheritdoc />
    public int Count => _items.Count;
}