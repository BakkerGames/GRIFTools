using static GRIFTools.GROD.GrodEnums;

namespace GRIFTools.GROD;

public class Grod
{
    public bool UseOverlay { get; set; } = false;

    public GrodItem? Get(string key)
    {
        key = NormalizeKey(key);
        GrodItem? item;
        if (UseOverlay && _overlay.TryGetValue(key, out item))
        {
            return item;
        }
        if (_base.TryGetValue(key, out item))
        {
            return item;
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
        if (item == null || item.Type == GrodItemType.Null || item.Value == null)
        {
            return "";
        }
        if (item.Type != GrodItemType.String)
        {
            throw new ArgumentException($"Value is not a string: {key}");
        }
        return (string)item.Value;
    }

    public void SetString(string key, string value)
    {
        var item = new GrodItem() { Type = GrodItemType.String, Value = value };
        Set(key, item);
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

    #region Private

    private readonly Dictionary<string, GrodItem?> _base = [];
    private readonly Dictionary<string, GrodItem?> _overlay = [];

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
            throw new ArgumentException("Invalid key");
        }
        return key;
    }

    #endregion
}
