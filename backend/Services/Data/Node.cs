class Node
{
    public BlockType _type { get; set; }
    public Dictionary<string, string>? _value { get; set; }

    public List<Node>? _nodes { get; set; }

    public Node(BlockType type, Dictionary<string, string>? value = null)
    {
        _type = type;
        _value = value;
    }

    public Node() { }

    public Node(List<Node> nodes)
    {
        _nodes = nodes;
    }

    public string toSql(string space = "", int siblingsCount = 0, Node prevBlock = null)
    {
        string _space = space;
        int _siblingsCount = siblingsCount > 0 ? siblingsCount : 0;
        string ret = "";
        //int cnt = 0;
        if (_nodes != null && _nodes.Count != 0)
            foreach (Node node in _nodes)
            {
                //_siblingsCount--;
                //if (node._type == BlockType.FIELD_NAME || node._type == BlockType.TABLE_NAME)
                //{
                //    if (node._nodes != null) _siblingsCount = node._nodes.Count;
                //    comma = _siblingsCount > 0 ? ", " : "";
                //}
                ret += node.typeToString(ref _space, prevBlock) + node.toSql(_space, _siblingsCount, node);
            }

        return ret;
    }

    private string typeToString(ref string space, Node prevBlock)
    {
        string ret = "", comma = "";
        switch (_type)
        {
            case BlockType.SELECT:
                ret += $"{space}SELECT ";
                break;
            case BlockType.FIELD_NAME:
                foreach (KeyValuePair<string, string> val in _value)
                {
                    string addition = "";
                    if (isAgregateFunc(prevBlock))
                    {
                        addition += $")" + (prevBlock._value.Values.First() != null ? $" as {prevBlock._value.Values.First()}" : "");
                    }
                    else if (prevBlock._type == BlockType.FIELD_NAME) comma = ", ";
                    ret += $"{comma}{val.Value}{addition}";
                }
                //if (_nodes != null && _nodes.Count != 0) ret += ", ";
                break;
            case BlockType.FROM:
                ret += $"<br/>{space}FROM ";
                break;
            case BlockType.TABLE_NAME:
                foreach (KeyValuePair<string, string> val in _value)
                {
                    if (prevBlock._type == BlockType.FIELD_NAME) comma = ", ";
                    ret += $"{comma}{val.Value}";
                }
                //if (_nodes != null && _nodes.Count != 0) ret += ", ";
                break;
            case BlockType.WHERE:
                ret += $"<br/>{space}WHERE ";
                break;
            case BlockType.CONDITION:
                foreach (KeyValuePair<string, string> val in _value)
                {
                    ret += $"{val.Value} ";
                }
                //ret += $"{_type} {_value}<br/>";
                if (_nodes != null && _nodes.Count != 0) space += "    ";
                break;
            case BlockType.AND:
                ret += $"<br/>AND ";
                break;
            case BlockType.OR:
                ret += $"<br/>OR ";
                break;
            case BlockType.COUNT:
                if (prevBlock._type == BlockType.FIELD_NAME) comma = ", ";
                ret += $"{comma}count(";
                break;
            case BlockType.MAX:
                if (prevBlock._type == BlockType.FIELD_NAME) comma = ", ";
                ret += $"{comma}max(";
                break;
            case BlockType.MIN:
                if (prevBlock._type == BlockType.FIELD_NAME) comma = ", ";
                ret += $"{comma}min(";
                break;
            case BlockType.SUM:
                if (prevBlock._type == BlockType.FIELD_NAME) comma = ", ";
                ret += $"{comma}sum(";
                break;
            case BlockType.AVG:
                if (prevBlock._type == BlockType.FIELD_NAME) comma = ", ";
                ret += $"{comma}avg(";
                break;
            case BlockType.ORDER_BY:
                ret += $"<br/>{space}ORDER BY ";
                break;
            case BlockType.GROUP_BY:
                ret += $"<br/>{space}GROUP BY ";
                break;
            case BlockType.HAVING:
                ret += $"<br/>{space}HAVING ";
                break;
        }
        return ret;
    }

    private bool isAgregateFunc(Node node)
    {
        return node._type == BlockType.COUNT || node._type == BlockType.MAX || node._type == BlockType.MIN ||
            node._type == BlockType.SUM || node._type == BlockType.AVG;
    }
}