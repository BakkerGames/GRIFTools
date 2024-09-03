using System.Text;
using static GRIFTools.GROD.GrodEnums;

namespace GRIFTools.GROD;

public class GrodItem
{
    public GrodItemType Type { get; set; }
    public GrodNumberType NumberType { get; set; }
    public object? Value { get; set; }

    public override string ToString()
    {
        StringBuilder result = new();
        result.Append('{');
        result.Append($"Type: {Type}, ");
        if (Type == GrodItemType.Number)
        {
            result.Append($"NumberType: {NumberType}, ");
        }
        result.Append("Value: ");
        if (Value == null)
        {
            result.Append("null");
        }
        else
        {
            switch (Type)
            {
                case GrodItemType.Null:
                    result.Append("null");
                    break;
                case GrodItemType.Bool:
                    result.Append((bool)Value ? "true" : "false");
                    break;
                case GrodItemType.String:
                    result.Append('"' + (string)Value + '"');
                    break;
                case GrodItemType.Number:
                    switch (NumberType)
                    {
                        case GrodNumberType.Int:
                            result.Append((int)Value);
                            break;
                        case GrodNumberType.Long:
                            result.Append((long)Value);
                            break;
                        case GrodNumberType.Float:
                            result.Append((float)Value);
                            break;
                        case GrodNumberType.Decimal:
                            result.Append((decimal)Value);
                            break;
                    };
                    break;
                case GrodItemType.List:
                    result.Append('[');
                    var itemList = (List<GrodItem>)Value;
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        if (i > 0) result.Append(',');
                        result.Append(itemList[i].ToString());
                    }
                    result.Append(']');
                    break;
                case GrodItemType.Obj:
                    result.Append('{');
                    var itemObj = (Dictionary<string, GrodItem?>)Value;
                    bool comma = false;
                    foreach (string objKey in itemObj.Keys)
                    {
                        if (comma)
                            result.Append(',');
                        else
                            comma = true;
                        result.Append('"');
                        result.Append(objKey);
                        result.Append("\":");
                        result.Append(itemObj[objKey]?.ToString() ?? "null");
                    }
                    result.Append('}');
                    break;
            }
        }
        result.Append('}');
        return result.ToString();
    }
}
