public class TableView
{
    private static int tableWidth = 150;

    public TableView() { }

    public string GetTable(List<Dictionary<string, string>> values)
    {
        string table = $"{GetLine(values.First().Count)}{GetRow(values.First(), true)}{GetLine(values.First().Count)}";
        foreach (Dictionary<string, string> value in values)
        {
            table += GetRow(value);
            table += GetLine(value.Count);
        }
        return table;
    }

    private string GetLine(int columnCount) {
        string res = "+";
        int space = (tableWidth - columnCount) / columnCount;
        for (int i = 0; i < columnCount; i++)
        {
            res += $"{new string('-', space)}+";
        }
        return $"{res}\n";
    }

    private string GetRow(Dictionary<string, string> columns, bool isName = false)
    {
        int width = (tableWidth - columns.Count) / columns.Count;
        string row = "|";

        foreach (var column in columns)
        {
            if (!isName) row += AlignCentre(column.Value, width) + "|";
            else row += AlignCentre(column.Key, width) + "|";
        }
        return $"{row}\n";
    }

    private string AlignCentre(string text, int width)
    {
        text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

        if (string.IsNullOrEmpty(text))
        {
            return new string(' ', width);
        }
        else
        {
            return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }
    }

    public TableDto GetJSONTable(List<Dictionary<string, string>> values)
    {
        List<string> columnNames = new List<string>(values.First().Keys);
        List<List<string>> columnValues = new List<List<string>>();

        foreach (Dictionary<string, string> column in values)
        {
            List<string> columnValue = new List<string>(column.Values);
            columnValues.Add(columnValue);
        }

        TableDto table = new TableDto()
        {
            columnNames = columnNames.ToArray(),
            columnValues = columnValues.Select(a => a.ToArray()).ToArray()
        };

        return table;
    }
}