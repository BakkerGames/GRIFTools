using System.Text;
using static GRIFTools.DagsConstants;

namespace GRIFTools;

/// <summary>
/// Generate a DAGS script object and assign its dictionary.
/// </summary>
public partial class Dags(Grod grod)
{
    /// <summary>
    /// Grod dictionary of (key,value) pairs
    /// </summary>
    public Grod Data { get; set; } = grod;

    /// <summary>
    /// Receives metadata from the calling program, such as text input.
    /// </summary>
    public Queue<string> InChannel { get; set; } = new();

    /// <summary>
    /// Sends metadata or commands back to the calling program.
    /// </summary>
    public Queue<string> OutChannel { get; set; } = new();

    /// <summary>
    /// Run one script and return any text in result.
    /// </summary>
    public void RunScript(string script, StringBuilder result)
    {
        if (string.IsNullOrWhiteSpace(script) || script.Equals(NULL_VALUE, OIC))
        {
            return;
        }
        if (!script.TrimStart().StartsWith('@'))
        {
            result.Append(script);
            return;
        }
        try
        {
            var tokens = SplitTokens(script);
            int index = 0;
            while (index < tokens.Length)
            {
                RunOneCommand(tokens, ref index, result);
            }
        }
        catch (Exception ex)
        {
            if (result.Length > 0) result.AppendLine();
            result.AppendLine($"{ex.Message}{Environment.NewLine}{script}");
        }
    }

    /// <summary>
    /// Format the script with line breaks and indents.
    /// Parameter "indent" adds one extra tab at the beginning of each line.
    /// </summary>
    public static string PrettyScript(string script, bool indent = false)
    {
        StringBuilder result = new();

        if (!script.TrimStart().StartsWith('@') && !script.TrimStart().StartsWith('['))
        {
            if (indent)
            {
                result.Append('\t');
            }
            result.Append(script);
            return result.ToString();
        }

        int startIndent = indent ? 1 : 0;
        int indentLevel = startIndent;
        int parens = 0;
        bool ifLine = false;
        bool forLine = false;
        bool forEachKeyLine = false;
        bool forEachListLine = false;
        bool inList = false;
        bool inArray = false;
        bool lastComma = false;
        var tokens = SplitTokens(script);

        foreach (string s in tokens)
        {
            // handle lists and arrays
            if (s == "[")
            {
                if (!inList)
                {
                    inList = true;
                    if (inArray && !lastComma)
                    {
                        result.AppendLine(",");
                    }
                }
                else
                {
                    inArray = true;
                    result.AppendLine();
                    indentLevel++;
                }
                if (indentLevel > 0)
                {
                    result.Append(new string('\t', indentLevel));
                }
                result.Append(s);
                lastComma = false;
                continue;
            }
            if (s == "]")
            {
                if (inList)
                {
                    inList = false;
                }
                else
                {
                    inArray = false;
                    if (!lastComma)
                    {
                        result.AppendLine();
                    }
                    if (indentLevel > startIndent) indentLevel--;
                    if (indentLevel > 0)
                    {
                        result.Append(new string('\t', indentLevel));
                    }
                }
                result.Append(s);
                lastComma = false;
                continue;
            }
            if (s == "," && inArray && !inList)
            {
                result.AppendLine(s);
                lastComma = true;
                continue;
            }
            if (inArray || inList)
            {
                result.Append(s);
                lastComma = false;
                continue;
            }
            // handle everything else
            switch (s)
            {
                case ELSEIF:
                    if (indentLevel > startIndent) indentLevel--;
                    break;
                case ELSE:
                    if (indentLevel > startIndent) indentLevel--;
                    break;
                case ENDIF:
                    if (indentLevel > startIndent) indentLevel--;
                    break;
                case ENDFOR:
                    if (indentLevel > startIndent) indentLevel--;
                    break;
                case ENDFOREACHKEY:
                    if (indentLevel > startIndent) indentLevel--;
                    break;
                case ENDFOREACHLIST:
                    if (indentLevel > startIndent) indentLevel--;
                    break;
            }
            if (parens == 0)
            {
                if (ifLine)
                {
                    result.Append(' ');
                }
                else
                {
                    if (result.Length > 0)
                    {
                        result.AppendLine();
                    }
                    if (indentLevel > 0)
                    {
                        result.Append(new string('\t', indentLevel));
                    }
                }
            }
            result.Append(s);
            switch (s)
            {
                case IF:
                    ifLine = true;
                    break;
                case ELSEIF:
                    ifLine = true;
                    break;
                case ELSE:
                    indentLevel++;
                    break;
                case THEN:
                    indentLevel++;
                    ifLine = false;
                    break;
                case FOR:
                    forLine = true;
                    break;
                case FOREACHKEY:
                    forEachKeyLine = true;
                    break;
                case FOREACHLIST:
                    forEachListLine = true;
                    break;
            }
            if (s.EndsWith('('))
            {
                parens++;
            }
            else if (s == ")")
            {
                if (parens > 0) parens--;
                if (forLine && parens == 0)
                {
                    forLine = false;
                    indentLevel++;
                }
                else if (forEachKeyLine && parens == 0)
                {
                    forEachKeyLine = false;
                    indentLevel++;
                }
                else if (forEachListLine && parens == 0)
                {
                    forEachListLine = false;
                    indentLevel++;
                }
            }
        }
        return result.ToString();
    }

    /// <summary>
    /// Format the script in a single line with minimal spaces.
    /// </summary>
    public static string CompressScript(string script)
    {
        if (!script.TrimStart().StartsWith('@'))
        {
            return script;
        }

        StringBuilder result = new();
        var tokens = SplitTokens(script);
        char lastChar = ',';
        bool addSpace;

        foreach (string s in tokens)
        {
            addSpace = false;
            if (s.StartsWith('@'))
            {
                if (lastChar != '(' && lastChar != ',')
                    addSpace = true;
            }
            if (addSpace)
            {
                result.Append(' ');
            }
            result.Append(s);
            lastChar = s[^1];
        }
        return result.ToString();
    }

    /// <summary>
    /// Expand a value containing a list into a list of strings
    /// </summary>
    public static List<string> ExpandList(string value)
    {
        int pos = 0;
        return ExpandList(value, ref pos);
    }

    /// <summary>
    /// Compress a list of strings into a value
    /// </summary>
    public static string CollapseList(List<string> list)
    {
        StringBuilder result = new();
        result.Append('[');
        bool addComma = false;
        foreach (string s in list)
        {
            if (addComma)
                result.Append(',');
            else
                addComma = true;
            var quote = false;
            foreach (char c in s)
            {
                switch (c)
                {
                    case ',':
                    case '"':
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                    case '\\':
                    case '[':
                    case ']':
                        quote = true;
                        break;
                }
                if (quote)
                    break;
            }
            var value = s;
            if (quote)
            {
                result.Append('"');
                value = value
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");
                result.Append(value);
                result.Append('"');
            }
            else if (value != NULL_VALUE)
            {
                result.Append(value);
            }
        }
        result.Append(']');
        return result.ToString();
    }

    /// <summary>
    /// Expand a value containing an array into a list of list of strings
    /// </summary>
    public static List<List<string>> ExpandArray(string list)
    {
        List<List<string>> result = [];
        int pos = 0;
        bool first = true;
        while (pos < list.Length)
        {
            char c = list[pos++];
            if (char.IsWhiteSpace(c)) continue;
            if (c == '[' && first)
            {
                first = false;
                continue;
            }
            if (c == ',') continue;
            if (c == ']') break;
            pos--;
            result.Add(ExpandList(list, ref pos));
        }
        return result;
    }

    /// <summary>
    /// Compress a list of list of strings into a value
    /// </summary>
    public static string CollapseArray(List<List<string>> list)
    {
        StringBuilder result = new();
        result.Append('[');
        var comma = false;
        foreach (var sublist in list)
        {
            if (comma)
                result.Append(',');
            else
                comma = true;
            result.Append(CollapseList(sublist));
        }
        result.Append(']');
        return result.ToString();
    }
}
