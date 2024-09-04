namespace GRIFTools;

public partial class Grod
{
    public string GetString(string key)
    {
        var value = Get(key);
        if (value == null)
        {
            return "";
        }
        return value.ToString() ?? "";
    }

    public void SetString(string key, string? value)
    {
        Set(key, value ?? "");
    }

    public int GetInt(string key)
    {
        var value = Get(key);
        if (value == null)
        {
            return 0;
        }
        if (int.TryParse(value.ToString(), out int result))
        {
            return result;
        }
        throw new SystemException($"Value is not an integer: {key}: {value}");
    }

    public void SetInt(string key, int? value)
    {
        Set(key, value ?? 0);
    }
}
