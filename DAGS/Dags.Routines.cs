using System.Text;
using static DAGSScriptLibrary.DAGSConstants;

namespace GRIFTools;

public partial class Dags
{
    private static int ConvertToInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals(NULL_VALUE, OIC) || value.Equals(FALSE_VALUE, OIC))
        {
            return 0;
        }
        if (value.Equals(TRUE_VALUE, OIC))
        {
            return 1;
        }
        if (int.TryParse(value.Trim(), out int answer))
        {
            return answer;
        }
        throw new SystemException($"Value is not numeric: {value}");
    }

    private static bool ConvertToBool(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Equals(NULL_VALUE, OIC) ||
            value.Equals(FALSE_VALUE, OIC) ||
            value.Equals("f", OIC) ||
            value.Equals("off", OIC) ||
            value.Equals("no", OIC) ||
            value.Equals("n", OIC) ||
            value == "0")
        {
            return false;
        }
        if (value.Equals(TRUE_VALUE, OIC) ||
            value.Equals("t", OIC) ||
            value.Equals("on", OIC) ||
            value.Equals("yes", OIC) ||
            value.Equals("y", OIC) ||
            value == "1" ||
            value == "-1")
        {
            return true;
        }
        throw new SystemException($"Value is not boolean: {value}");
    }

    private static string ConvertToBoolString(bool value)
    {
        return value ? TRUE_VALUE : FALSE_VALUE;
    }

    private static void CheckParamCount(string token, List<string> paramList, int expected)
    {
        if (paramList.Count == expected)
            return;
        throw new SystemException($"Incorrect number of parameters: {token}({expected}) - Found: {paramList.Count}");
    }

    private static void CheckParamCountAtLeast(string token, List<string> paramList, int expected)
    {
        if (paramList.Count >= expected)
            return;
        throw new SystemException($"Incorrect number of parameters: {token}({expected}) - Found: {paramList.Count}");
    }

    private static List<string> ExpandList(string value, ref int pos)
    {
        List<string> result = [];
        bool inQuote = false;
        bool lastSlash = false;
        StringBuilder token = new();
        value = value.Trim();
        while (pos < value.Length)
        {
            char c = value[pos++];
            if (token.Length == 0 && char.IsWhiteSpace(c))
            {
                continue;
            }
            if (!inQuote && c == ']')
            {
                break;
            }
            if (!inQuote && c == '[')
            {
                if (token.Length > 0)
                {
                    throw new SystemException($"Unexpected character within list: {c}");
                }
                continue;
            }
            if (inQuote)
            {
                if (c == '\\' && !lastSlash)
                {
                    lastSlash = true;
                    continue;
                }
                if (lastSlash)
                {
                    switch (c)
                    {
                        case 't':
                            token.Append('\t');
                            break;
                        case 'n':
                            token.Append('\n');
                            break;
                        case 'r':
                            token.Append('\r');
                            break;
                        default:
                            token.Append(c);
                            break;
                    }
                    lastSlash = false;
                    continue;
                }
                if (c == '"')
                {
                    inQuote = false;
                    continue;
                }
                token.Append(c);
                continue;
            }
            if (c == ',')
            {
                if (token.Length >= 2 && token[0] == '"' && token[^1] == '"')
                {
                    result.Add(token.ToString()[1..^1]);
                }
                else
                {
                    result.Add(token.ToString().Trim());
                }
                token.Clear();
                continue;
            }
            token.Append(c);
        }
        if (token.Length >= 2 && token[0] == '"' && token[^1] == '"')
        {
            result.Add(token.ToString()[1..^1]);
        }
        else if (result.Count > 0 || token.Length > 0)
        {
            result.Add(token.ToString().Trim());
        }
        return result;
    }
}
