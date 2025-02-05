using System.Text;

namespace GRIFTools;

public partial class Grod
{
    private const string Id_1D = "1D_Array";
    private const string Id_2D = "2D_Array";
    private const string Id_3D = "3D_Array";

    /// <summary>
    /// Create a 1D array of strings
    /// </summary>
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

    /// <summary>
    /// Get a 1D array of strings
    /// </summary>
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

    /// <summary>
    /// Set a 1D array value to an array of strings
    /// </summary>
    public void Set1D(string key, string[] data)
    {
        key = NormalizeKey(key);
        var id = Get(key);
        if (id.StartsWith(Id_1D) && int.TryParse(id[(Id_1D.Length + 1)..^1], out int count))
        {
            if (data.Length == count)
            {
                var testData = string.Join(' ', data);
                if (!testData.Contains(',')) // check that no commas in data
                {
                    Set($"{key}_0", string.Join(',', data));
                    return;
                }
            }
        }
        throw new SystemException($"Set1D: Invalid 1D array or data: {key}");
    }

    /// <summary>
    /// Create a 2D array of strings
    /// </summary>
    public void Create2D(string key, int height, int width)
    {
        key = NormalizeKey(key);
        if (height <= 0 || width <= 0)
        {
            throw new SystemException($"Create2D: Invalid height or width: {height} {width}");
        }
        Set(key, $"{Id_2D}[{height},{width}]");
        for (int y = 0; y < height; y++)
        {
            Set($"{key}_{y}", new string(',', width - 1));
        }
    }

    /// <summary>
    /// Get a 2D array of strings
    /// </summary>
    public string[,] Get2D(string key)
    {
        try
        {
            key = NormalizeKey(key);
            var id = Get(key);
            if (!id.StartsWith(Id_2D))
                throw new SystemException();
            var dimensions = id[(Id_2D.Length + 1)..^1].Split(',');
            var height = int.Parse(dimensions[0]);
            var width = int.Parse(dimensions[1]);
            var result = new string[height, width];
            for (int y = 0; y < height; y++)
            {
                var line = Get($"{key}_{y}").Split(',');
                for (int x = 0; x < width; x++)
                {
                    result[y, x] = line[x];
                }
            }
            return result;
        }
        catch (Exception)
        {
            throw new SystemException($"Get2D: Invalid 2D array: {key}");
        }
    }

    /// <summary>
    /// Set a 2D array value to an array of strings
    /// </summary>
    public void Set2D(string key, string[,] data)
    {
        try
        {
            key = NormalizeKey(key);
            var id = Get(key);
            if (!id.StartsWith(Id_2D))
                throw new SystemException();
            var dimensions = id[(Id_2D.Length + 1)..^1].Split(',');
            var height = int.Parse(dimensions[0]);
            var width = int.Parse(dimensions[1]);
            for (int y = 0; y < height; y++)
            {
                StringBuilder line = new();
                for (int x = 0; x < width; x++)
                {
                    if (x > 0) line.Append(',');
                    if (data[y, x].Contains(','))
                        throw new SystemException();
                    line.Append(data[y, x]);
                }
                Set($"{key}_{y}", line.ToString());
            }
        }
        catch (Exception)
        {
            throw new SystemException($"Set2D: Invalid 2D array or data: {key}");
        }
    }

    /// <summary>
    /// Create a 3D array of strings
    /// </summary>
    public void Create3D(string key, int height, int width, int depth)
    {
        key = NormalizeKey(key);
        if (height <= 0 || width <= 0 || depth <= 0)
        {
            throw new SystemException($"Create3D: Invalid height, width, or depth: {height} {width} {depth}");
        }
        Set(key, $"{Id_3D}[{height},{width},{depth}]");
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Set($"{key}_{y}_{x}", new string(',', depth - 1));
            }
        }
    }

    /// <summary>
    /// Get a 3D array of strings
    /// </summary>
    public string[,,] Get3D(string key)
    {
        try
        {
            key = NormalizeKey(key);
            var id = Get(key);
            if (!id.StartsWith(Id_3D))
                throw new SystemException();
            var dimensions = id[(Id_3D.Length + 1)..^1].Split(',');
            var height = int.Parse(dimensions[0]);
            var width = int.Parse(dimensions[1]);
            var depth = int.Parse(dimensions[2]);
            var result = new string[height, width, depth];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var line = Get($"{key}_{y}_{x}").Split(',');
                    for (int z = 0; z < depth; z++)
                    {
                        result[y, x, z] = line[z];
                    }
                }
            }
            return result;
        }
        catch (Exception)
        {
            throw new SystemException($"Get3D: Invalid 3D array: {key}");
        }
    }

    /// <summary>
    /// Set a 3D array value to an array of strings
    /// </summary>
    public void Set3D(string key, string[,,] data)
    {
        try
        {
            key = NormalizeKey(key);
            var id = Get(key);
            if (!id.StartsWith(Id_3D))
                throw new SystemException();
            var dimensions = id[(Id_3D.Length + 1)..^1].Split(',');
            var height = int.Parse(dimensions[0]);
            var width = int.Parse(dimensions[1]);
            var depth = int.Parse(dimensions[2]);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    StringBuilder line = new();
                    for (int z = 0; z < depth; z++)
                    {
                        if (z > 0) line.Append(',');
                        if (data[y, x, z].Contains(','))
                            throw new SystemException();
                        line.Append(data[y, x, z]);
                    }
                    Set($"{key}_{y}_{x}", line.ToString());
                }
            }
        }
        catch (Exception)
        {
            throw new SystemException($"Set3D: Invalid 3D array or data: {key}");
        }
    }
}
