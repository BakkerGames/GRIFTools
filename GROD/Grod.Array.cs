namespace GRIFTools;

public partial class Grod
{
    private const string Id_1D = "1D_Array";
    //private const string Id_2D = "2D_Array";
    //private const string Id_3D = "3D_Array";

    public void Create1D(string key, int count)
    {
        key = NormalizeKey(key);
        if (count <= 0)
        {
            throw new SystemException($"Create1D: Invalid count {count}");
        }
        Set(key, $"{Id_1D}[{count}]");
        Set($"{key}_0", new string(',', count - 1));
    }

    public string[] Get1D(string key)
    {
        key = NormalizeKey(key);
        var id = Get(key);
        if (id.StartsWith(Id_1D) && int.TryParse(id[(Id_1D.Length + 1)..^1], out int count))
        {
            var result = Get($"{key}_0").Split(',');
            if (result.Length == count)
            {
                return result.ToArray();
            }
        }
        throw new SystemException($"Get1D: Invalid 1D array: {key}");
    }

    public void Set1D(string key, string[] data)
    {
        key = NormalizeKey(key);
        var id = Get(key);
        if (id.StartsWith(Id_1D) && int.TryParse(id[(Id_1D.Length + 1)..^1], out int count))
        {
            if (data.Length == count)
            {
                Set($"{key}_0", string.Join(',', data));
            }
        }
        throw new SystemException($"Set1D: Invalid 1D array: {key}");
    }

    /*
    public void Create2D(string key, int height, int width)
    {
        key = NormalizeKey(key);
    }

    public string[,] Get2D(string key)
    {
        key = NormalizeKey(key);
    }

    public void Set2D(string key, string[,] data)
    {
        key = NormalizeKey(key);
    }

    public void Create3D(string key, int height, int width, int depth)
    {
        key = NormalizeKey(key);
    }

    public string[,,] Get3D(string key)
    {
        key = NormalizeKey(key);
    }

    public void Set3D(string key, string[,,] data)
    {
        key = NormalizeKey(key);
    }
    */
}
