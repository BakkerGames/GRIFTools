namespace GRIFTools;

public partial class Dags
{
    /// <summary>
    /// Gets a value from the dictionary, or "" if not found.
    /// </summary>
    private string Get(string key)
    {
        return Data.GetString(key);
    }

    /// <summary>
    /// Sets a value into the dictionary.
    /// </summary>
    private void Set(string key, string value)
    {
        Data.SetString(key, value);
    }

    /// <summary>
    /// Gets a value from the dictionary and converts it to an integer.
    /// </summary>
    private int GetInt(string key)
    {
        var item = Data.Get(key);
        if (item == null || item.Type == GROD.GrodEnums.GrodItemType.Null || item.Value == null)
        {
            return 0;
        }
        if (item.Type == GROD.GrodEnums.GrodItemType.Number && item.NumberType == GROD.GrodEnums.GrodNumberType.Int)
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
}
