using System.Text;
using static GRIFTools.GrodEnums;

namespace GRIFTools;

public class Grod
{
    public bool UseOverlay { get; set; } = false;

    public GrodItem? Get(string key)
    {
        key = NormalizeKey(key);
        object? item;
        if (UseOverlay && _overlay.TryGetValue(key, out item))
        {
            if (item == null)
            {
                return null;
            }
            if (item is GrodItem)
            {
                return (GrodItem)item;
            }
            throw new ArgumentException($"Item is not type GrodItem: {item.GetType()}");
        }
        if (_base.TryGetValue(key, out item))
        {
            if (item == null)
            {
                return null;
            }
            if (item is GrodItem)
            {
                return (GrodItem)item;
            }
            throw new ArgumentException($"Item is not type GrodItem: {item.GetType()}");
        }
        return null;
    }

    public void Set(string key, GrodItem? item)
    {
        key = NormalizeKey(key);
        if (UseOverlay)
        {
            if (!_overlay.TryAdd(key, item))
            {
                _overlay[key] = item;
            }
        }
        else if (!_base.TryAdd(key, item))
        {
            _base[key] = item;
        }
    }

    public string GetString(string key)
    {
        var item = Get(key);
        if (item == null || item.Value == null)
        {
            return "";
        }
        if (item.Type == GrodItemType.String)
        {
            return (string)item.Value;
        }
        if (item.Type == GrodItemType.List)
        {
            return ListToString((List<GrodItem>?)item.Value);
        }
        throw new ArgumentException($"Value is not a string: {key}");
    }

    public void SetString(string key, string? value)
    {
        var item = new GrodItem() { Type = GrodItemType.String, Value = value ?? "" };
        Set(key, item);
    }

    public List<GrodItem>? GetList(string key)
    {
        key = NormalizeKey(key);
        object? item;
        if (!UseOverlay || !_overlay.TryGetValue(key, out item))
        {
            if (!_base.TryGetValue(key, out item))
            {
                return null;
            }
        }
        if (item == null)
        {
            return null;
        }
        if (item.GetType() != typeof(GrodItem) || ((GrodItem)item).Type != GrodItemType.List)
        {
            throw new ArgumentException($"Value is not a list: {key}");
        }
        var value = ((GrodItem)item).Value;
        return (List<GrodItem>?)value;
    }

    public void SetList(string key, List<GrodItem>? value)
    {
        key = NormalizeKey(key);
        var listValue = new GrodItem() { Type = GrodItemType.List, Value = value };
        if (UseOverlay)
        {
            if (!_overlay.TryAdd(key, listValue))
            {
                _overlay[key] = listValue;
            }
        }
        else
        {
            if (!_base.TryAdd(key, listValue))
            {
                _base[key] = listValue;
            }
        }
    }

    public void Clear(WhichData whichData = WhichData.Both)
    {
        if (whichData == WhichData.Both || whichData == WhichData.Base)
        {
            _base.Clear();
        }
        if (whichData == WhichData.Both || whichData == WhichData.Overlay)
        {
            _overlay.Clear();
        }
    }

    public List<string> Keys(WhichData whichData = WhichData.Both)
    {
        if (!UseOverlay || whichData == WhichData.Base)
        {
            return _base.Keys.ToList();
        }
        if (UseOverlay && whichData == WhichData.Overlay)
        {
            return _overlay.Keys.ToList();
        }
        return _base.Keys.Union(_overlay.Keys).ToList();
    }

    public int Count(WhichData whichData = WhichData.Both)
    {
        return Keys(whichData).Count;
    }

    public bool ContainsKey(string key)
    {
        key = NormalizeKey(key);
        if (_base.ContainsKey(key))
        {
            return true;
        }
        if (UseOverlay && _overlay.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    public void Remove(string key)
    {
        key = NormalizeKey(key);
        _base.Remove(key);
        if (UseOverlay)
        {
            _overlay.Remove(key);
        }
    }

    public void Revert(string key)
    {
        key = NormalizeKey(key);
        if (UseOverlay)
        {
            _overlay[key] = _base[key];
        }
    }

    public void MergeOverlay()
    {
        foreach (string key in Keys(WhichData.Overlay))
        {
            var value = _overlay[key];
            if (!_base.TryAdd(key, value))
            {
                _base[key] = value;
            }
        }
        _overlay.Clear();
    }

    public static string ListToString(List<GrodItem>? list)
    {
        StringBuilder result = new();
        result.Append('[');
        var comma = false;
        foreach (GrodItem listItem in list ?? [])
        {
            if (comma)
            {
                result.Append(',');
            }
            else
            {
                comma = true;
            }
            if (listItem.Value != null)
            {
                var value = listItem.Value.ToString() ?? "";
                var needsQuotes = false;
                foreach (char c in value)
                {
                    if (char.IsWhiteSpace(c) || c == ',')
                    {
                        needsQuotes = true;
                    }
                }
                if (needsQuotes)
                {
                    result.Append('"');
                    result.Append(value);
                    result.Append('"');
                }
                else
                {
                    result.Append(value);
                }
            }
        }
        result.Append(']');
        return result.ToString();
    }

    public static List<GrodItem>? StringToList(string value)
    {
        List<GrodItem> result = [];
        StringBuilder item = new();
        var inQuote = false;
        foreach (char c in value[1..])
        {
            if (inQuote)
            {
                if (c == '"')
                {
                    inQuote = false;
                }
                else
                {
                    item.Append(c);
                }
            }
            else if (c == '"' && item.Length == 0)
            {
                inQuote = true;
            }
            else if (c == ',' || c == ']')
            {
                result.Add(new GrodItem() { Type = GrodItemType.String, Value = item.ToString() });
                item.Clear();
            }
            else
            {
                item.Append(c);
            }
        }
        return result;
    }

    #region Private

    private readonly Dictionary<string, object?> _base = [];
    private readonly Dictionary<string, object?> _overlay = [];

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }
        key = key.ToLower().Trim();
        foreach (char c in key)
        {
            if (c >= 'a' && c <= 'z') continue;
            if (c >= '0' && c <= '9') continue;
            if (c == '_' || c == '.') continue;
            if (c == '@' || c == '(' || c == ')' || c == ',') continue;
            if (c == '*' || c == '#' || c == '?') continue;
            throw new ArgumentException($"Invalid key: {key}");
        }
        return key;
    }

    #endregion
}
