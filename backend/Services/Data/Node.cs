class Node
{
    public BlockType _type { get; set; }
    public string? _value { get; set; } = "";

    public List<Node>? _nodes { get; set; }

    public Node(BlockType type, string? value = "")
    {
        _type = type;
        _value = value;
    }

    public Node() { }

    public Node(List<Node> nodes)
    {
        _nodes = nodes;
    }

    public string toSql(string space = "", int siblingsCount = 0)
    {
        string _space = space;
        int _siblingsCount = siblingsCount > 0 ? siblingsCount : 0;
        string ret = "", comma = "";
        //int cnt = 0;
        if (_nodes != null && _nodes.Count != 0)
            foreach (Node node in _nodes)
            {
                _siblingsCount--;
                if (node._type == BlockType.FIELD_NAME || node._type == BlockType.TABLE_NAME)
                {
                    if (node._nodes != null) _siblingsCount = node._nodes.Count;
                    comma = _siblingsCount > 0 ? ", " : "";
                }
                ret += node.typeToString(ref _space, comma) + node.toSql(_space, _siblingsCount);
            }

        return ret;
    }

    private string typeToString(ref string space, string comma)
    {
        string ret = "";
        switch (_type)
        {
            case BlockType.SELECT:
                ret += $"{space}SELECT ";
                break;
            case BlockType.FIELD_NAME:
                ret += $"{_type} {_value}{comma}";
                //if (_nodes != null && _nodes.Count != 0) ret += ", ";
                break;
            case BlockType.FROM:
                ret += $"<br/>{space}FROM ";
                break;
            case BlockType.TABLE_NAME:
                ret += $"{_type} {_value}{comma}";
                //if (_nodes != null && _nodes.Count != 0) ret += ", ";
                break;
            case BlockType.WHERE:
                ret += $"<br/>{space}WHERE ";
                break;
            case BlockType.CONDITION:
                ret += $"{_type} {_value}<br/>";
                if (_nodes != null && _nodes.Count != 0) space += "    ";
                break;

        }
        return ret;
    }
}