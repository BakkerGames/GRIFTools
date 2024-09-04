using GRIFTools;
using static GRIFTools.DagsConstants;
using static GRIFTools.GrodEnums;

namespace GRIFTools;

public partial class Dags
{
    /// <summary>
    /// Gets a value from the dictionary, or "" if not found.
    /// </summary>
    private string Get(string key)
    {
        var result = Data.GetString(key) ?? "";
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
        var item = Data.Get(key);
        if (item == null || item.Value == null)
        {
            return 0;
        }
        if (item.Type == GrodItemType.Number && item.NumberType == GrodNumberType.Int)
        {
            return (int)item.Value;
        }
        try
        {
            return ConvertToInt(item.Value.ToString() ?? "0");
        }
        catch (Exception)
        {
            throw new SystemException($"Value is not numeric: {key}: {item.Value}");
        }
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
        var itemList = Data.GetList(key);
        if (itemList == null)
        {
            return [];
        }
        List<string> result = [];
        foreach (GrodItem g in itemList)
        {
            result.Add(g?.Value?.ToString() ?? "");
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
        var item = Data.Get(key);
        if (item == null || item.Value == null)
        {
            return "";
        }
        if (item.Type != GrodItemType.List)
        {
            throw new ArgumentException($"Item is not a list: {key}");
        }
        var itemList = (List<GrodItem>)item.Value;
        if (itemList == null || itemList.Count <= index)
        {
            return "";
        }
        var indexItem = (GrodItem?)itemList[index];
        if (indexItem == null)
        {
            return "";
        }
        if (indexItem.Type != GrodItemType.String)
        {
            throw new ArgumentException($"Item is not a string: {key}[{index}]");
        }
        return (string?)indexItem.Value ?? "";
    }

    /// <summary>
    /// Sets Data[key] to a list of strings.
    /// </summary>
    private void SetList(string key, List<string>? itemList)
    {
        List<GrodItem> grodItems = [];
        foreach (string s in itemList ?? [])
        {
            grodItems.Add(new GrodItem() { Type = GrodItemType.String, Value = s });
        }
        Data.SetList(key, grodItems);
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
        var itemList = Data.GetList(key) ?? [];
        while (itemList.Count <= index)
        {
            itemList.Add(new GrodItem() { Type = GrodItemType.String, Value = "" });
        }
        if (itemList[index] != null && itemList[index].Type == GrodItemType.String)
        {
            itemList[index].Value = value;
        }
        else
        {
            itemList[index] = new GrodItem() { Type = GrodItemType.String, Value = value };
        }
        Data.SetList(key, itemList);
    }

    private List<List<string>> GetArray(string key)
    {
        var array = Data.Get(key);
        if (array == null)
        {
            return [];
        }
        if (array.Type != GrodItemType.List)
        {
            throw new ArgumentException($"Item is not an array: {key}");
        }
        if (array.Value == null)
        {
            return [];
        }
        List<List<string>> result = [];
        foreach (GrodItem gRow in (List<GrodItem>)array.Value)
        {
            List<string> row = [];
            if (gRow != null && gRow.Value != null)
            {
                if (gRow.Type != GrodItemType.List)
                {
                    throw new ArgumentException($"Item is not an array: {key}");
                }
                foreach (GrodItem gCol in (List<GrodItem>)gRow.Value)
                {
                    row.Add(gCol?.Value?.ToString() ?? "");
                }
            }
            result.Add(row);
        }
        return result;
    }

    private void SetArray(string key, List<List<string>> array)
    {
        List<GrodItem> grodArray = [];
        foreach (List<string> row in array)
        {
            List<GrodItem> gRow = [];
            foreach (string col in row)
            {
                gRow.Add(new GrodItem() { Type = GrodItemType.String, Value = col });
            }
            grodArray.Add(new GrodItem() { Type = GrodItemType.List, Value = gRow });
        }
        GrodItem item = new()
        {
            Type = GrodItemType.List,
            Value = grodArray
        };
        Data.Set(key, item);
    }
}
