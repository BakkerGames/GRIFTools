using static GRIFTools.DagsConstants;

namespace GRIFTools;

public partial class Dags
{
    /// <summary>
    /// Gets a value from the dictionary, or "" if not found.
    /// </summary>
    private string Get(string key)
    {
        var result = Data.GetString(key);
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
        Data.SetString(key, value);
    }

    /// <summary>
    /// Gets a value from the dictionary and converts it to an integer.
    /// </summary>
    private int GetInt(string key)
    {
        return Data.GetInt(key);
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
    /// Get a list of strings.
    /// </summary>
    private List<string> GetList(string key)
    {
        var itemList = Data.Get(key);
        if (itemList == null)
        {
            return [];
        }
        if (itemList.GetType() != typeof(List<string>))
        {
            throw new SystemException($"Value is not a list: {key}");
        }
        return (List<string>)itemList;
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
        var itemList = GetList(key);
        if (index >= itemList.Count)
        {
            return "";
        }
        var value = itemList[index];
        if (value == null || value == NULL_VALUE)
        {
            return "";
        }
        return value;
    }

    /// <summary>
    /// Sets Data[key] to a list of strings.
    /// </summary>
    private void SetList(string key, List<string>? itemList)
    {
        Data.Set(key, itemList);
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
    private void SetListItem(string key, int index, string value)
    {
        var itemList = GetList(key) ?? [];
        while (itemList.Count <= index)
        {
            itemList.Add("");
        }
        itemList[index] = value;
        SetList(key, itemList);
    }

    /// <summary>
    /// Returns a 2-D array of strings.
    /// </summary>
    private List<List<string>> GetArray(string key)
    {
        var itemArray = Data.Get(key);
        if (itemArray == null)
        {
            return [];
        }
        if (itemArray.GetType() != typeof(List<List<string>>))
        {
            throw new SystemException($"Value is not an array: Key = {key}");
        }
        return (List<List<string>>)itemArray;
    }

    private string GetArrayItem(string key, int y, int x)
    {
        var array = GetArray(key);
        if (array.Count <= y || array[y].Count <= x)
        {
            return "";
        }
        var value = array[y][x];
        if (value == null || value == NULL_VALUE)
        {
            return "";
        }
        return value;
    }

    /// <summary>
    /// Sets Data[key] to a 2-D array of strings.
    /// </summary>
    private void SetArray(string key, List<List<string>>? array)
    {
        Data.Set(key, array);
    }

    /// <summary>
    /// Sets an item in a 2-D array to a value.
    /// </summary>
    private void SetArrayItem(string key, int y, int x, string value)
    {
        var array = GetArray(key);
        while (array.Count <= y)
        {
            array.Add([]);
        }
        while (array[y].Count <= x)
        {
            array[y].Add("");
        }
        array[y][x] = value;
        SetArray(key, array);
    }
}
