using static GRIFTools.GrodEnums;

namespace GRIFTools;

public partial class Grod
{
    public bool UseOverlay { get; set; } = false;

    public object? Get(string key)
    {
        key = NormalizeKey(key);
        object? item;
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

    public void Set(string key, object? item)
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

    public void Clear(WhichData which = WhichData.Both)
    {
        if (which == WhichData.Both || which == WhichData.Base)
        {
            _base.Clear();
        }
        if (which == WhichData.Both || which == WhichData.Overlay)
        {
            _overlay.Clear();
        }
    }

    public List<string> Keys(WhichData which = WhichData.Both)
    {
        if (!UseOverlay || which == WhichData.Base)
        {
            return _base.Keys.ToList();
        }
        if (UseOverlay && which == WhichData.Overlay)
        {
            return _overlay.Keys.ToList();
        }
        return _base.Keys.Union(_overlay.Keys).ToList();
    }

    public int Count(WhichData which = WhichData.Both)
    {
        // count the Keys so duplicates are only counted once
        return Keys(which).Count;
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
        _overlay.Remove(key);
    }
}
