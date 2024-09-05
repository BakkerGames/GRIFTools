using System;
using System.Reflection;
using static GRIFTools.DagsConstants;

namespace GRIFTools;

public partial class Dags
{
    /// <summary>
    /// Gets a value from the dictionary, or "" if not found.
    /// </summary>
    private string Get(string key)
    {
        var result = Data.Get(key);
        if (result == NULL_VALUE)
        {
            result = "";
        }
        return result;
    }

    /// <summary>
    /// Sets a value into the dictionary.
    /// </summary>
    private void Set(string key, string value)
    {
        if (value == NULL_VALUE)
        {
            value = "";
        }
        Data.Set(key, value);
    }

    /// <summary>
    /// Gets a value from the dictionary and converts it to an integer.
    /// </summary>
    private int GetInt(string key)
    {
        var value = Data.Get(key);
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        throw new SystemException($"Value is not an int: {key}: {value}");
    }

    /// <summary>
    /// Gets a subset of the dictionary where key begins with the prefix.
    /// </summary>
    private Dictionary<string, string?> GetByPrefix(string prefix)
    {
        Dictionary<string, string?> result = [];
        List<string> keys;
        keys = Data.Keys().Where(x => x.StartsWith(prefix, OIC)).ToList();
        foreach (string k in keys)
        {
            result.Add(k, Get(k));
        }
        return result;
    }

    /// <summary>
    /// Get an item from a list of strings.
    /// </summary>
    private string GetListItem(string key, string index)
    {
        if (!int.TryParse(index, out int tempIndex))
        {
            throw new ArgumentException($"Value is not an integer: {index}");
        }
        return GetListItem(key, tempIndex);
    }

    /// <summary>
    /// Get an item from a list of strings.
    /// </summary>
    private string GetListItem(string key, int index)
    {
        var itemKey = $"{key}.{index}";
        return Data.Get(itemKey);
    }

    /// <summary>
    /// Sets an item in a list to a value.
    /// </summary>
    private void SetListItem(string key, string index, string value)
    {
        if (!int.TryParse(index, out int tempIndex))
        {
            throw new ArgumentException($"Value is not an integer: {index}");
        }
        SetListItem(key, tempIndex, value);
    }

    /// <summary>
    /// Sets an item in a list to a value.
    /// </summary>
    private void SetListItem(string key, int index, string? value)
    {
        if (index < 0)
        {
            throw new SystemException($"List index cannot be negative: {key}: {index}");
        }
        var maxKey = $"{key}.max";
        var max = Data.Get(maxKey);
        if (max == "" || !int.TryParse(max, out int maxValue) || maxValue < index)
        {
            Data.Set(maxKey, index.ToString());
        }
        var itemKey = $"{key}.{index}";
        Data.Set(itemKey, value);
    }

    private void AddListItem(string key, string? value)
    {
        var maxKey = $"{key}.max";
        var max = Data.Get(maxKey);
        var index = 0;
        if (max != "" && int.TryParse(max, out int maxValue))
        {
            index = maxValue + 1;
        }
        Data.Set(maxKey, index.ToString());
        var itemKey = $"{key}.{index}";
        Data.Set(itemKey, value);
    }

    private string GetArrayItem(string key, int y, int x)
    {
        var itemKey = $"{key}.{y}.{x}";
        return Data.Get(itemKey);
    }

    /// <summary>
    /// Sets an item in a 2-D array to a value.
    /// </summary>
    private void SetArrayItem(string key, int y, int x, string value)
    {
        if (y < 0 || x < 0)
        {
            throw new SystemException($"Array indexes cannot be negative: {key}: {y},{x}");
        }
        var yMaxKey = $"{key}.y.max";
        var yMax = Data.Get(yMaxKey);
        if (yMax == "" || !int.TryParse(yMax, out int yMaxValue) || yMaxValue < y)
        {
            Data.Set(yMaxKey, y.ToString());
        }
        var xMaxKey = $"{key}.x.max";
        var xMax = Data.Get(xMaxKey);
        if (xMax == "" || !int.TryParse(xMax, out int xMaxValue) || xMaxValue < x)
        {
            Data.Set(xMaxKey, x.ToString());
        }
        var itemKey = $"{key}.{y}.{x}";
        Data.Set(itemKey, value);
    }
}
