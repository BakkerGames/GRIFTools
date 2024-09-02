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
        StringBuilder result;
        if (Value == null)
            return "null";
        switch (Type)
        {
            case GrodItemType.Null:
                return "null";
            case GrodItemType.Bool:
                return (bool)Value ? "true" : "false"; 
            case GrodItemType.String:
                return '"' + (string)Value + '"';
            case GrodItemType.Number:
                return NumberType switch
                {
                    GrodNumberType.Int => ((int)Value).ToString(),
                    GrodNumberType.Long => ((long)Value).ToString(),
                    GrodNumberType.Float => ((float)Value).ToString(),
                    GrodNumberType.Decimal => ((decimal)Value).ToString(),
                    _ => "0",
                };
            case GrodItemType.List:
                result = new();
                result.Append('[');
                var itemList = (List<GrodItem>)Value;
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (i > 0) result.Append(',');
                    result.Append(itemList[i].ToString());
                }
                result.Append(']');
                return result.ToString();
            case GrodItemType.Obj:
                result = new();
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
                return result.ToString();
        }
        return "";
    }
}
