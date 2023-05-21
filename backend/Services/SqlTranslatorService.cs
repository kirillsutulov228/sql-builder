public class SqlTranslatorService
{
    public (String, ErrorNode) Translate(List<RawQueryNode> rawQueryNodes)
    {
        LexParser parser = new LexParser();
        List<Lexem> tokens = parser.Parse(rawQueryNodes);
        SyntAnalyzer syntAnalyzer = new SyntAnalyzer();
        Node syntTree = syntAnalyzer.Analyze(tokens);
        Console.WriteLine(syntTree.toSql());
        Console.WriteLine(syntAnalyzer.error.Item2);
        ErrorNode error = new ErrorNode();
        error.blockId = syntAnalyzer.error.Item1;
        error.message = syntAnalyzer.error.Item2;

        return new (syntTree.toSql(), error);
    }
}

class Lexem
{
    public BlockType _type { get; set; }
    public string _id { get; set; }
    public string? _value { get; set; }

    public Lexem(BlockType type, string id, string? value = null)
    {
        _type = type;
        _id = id;
        _value = value;
    }

    public Lexem() { }
}
class LexParser
{
    private string _blockId = "";
    private List<Lexem> _lexems = new List<Lexem> ();
    private string _parentId = "";

    public LexParser() { }

    public List<Lexem> Parse(List<RawQueryNode> rawQueryNodes)
    {
        foreach(RawQueryNode rawQueryNode in rawQueryNodes)
        {
            getLexems(rawQueryNode);
        }
        return _lexems;
    }

    private void getLexems(RawQueryNode curNode)
    {
        if (curNode == null) return;
        _blockId = curNode.Id;
        _parentId = curNode.ParentId;
        _lexems.Add(new Lexem(findType(curNode.BlockType), curNode.Id));
        if (curNode.Children == null) return;
        foreach (RawQueryNode node in curNode.Children)
        {
            getLexems(node);
        }
    }

    private BlockType findType(string type)
    {
        var res = Constants.Types.ContainsKey(type);
        if (res) return (BlockType) Constants.Types[type];
        return BlockType.ERROR;
    }
}
 
class SyntAnalyzer
{
    List<Lexem> _tokens = new List<Lexem>();
    Lexem _curToken, _prevToken;
    public Tuple<string, string> error = new Tuple<string, string>("", "");

    public SyntAnalyzer() { }

    public Node Analyze(List<Lexem> lexems)
    {
        _tokens = lexems;
        NextToken();
        Node tree = Block();
        return tree;
    }

    private void NextToken()
    {
        if (_tokens != null && _tokens.Count != 0)
        {
            _prevToken = _curToken;
            _curToken = _tokens.First();
            _tokens.Remove(_curToken);
        }
        else
        {
            _prevToken = _curToken;
            Reset();
        }
    }

    private void Reset()
    {
        _tokens = new List<Lexem>();
        _curToken = new Lexem();
    }
    private void Expect(BlockType type)
    {
        if (_curToken._type == type)
        {
            NextToken();
        }
        else
        {
            if (_curToken._id == null)
            {
                _prevToken = _curToken;
                Error("-1");
            }
            else Error(_curToken._id);
        }
    }

    private void Error(string s)
    {
        if (s == "-1")
        {
            error = new Tuple<string, string>(s, "Ожидаются недостающие команды.");
        }
        else
        {
            string errType = Constants.Types.FirstOrDefault(x => x.Value == (int)_curToken._type).Key;
            error = new Tuple<string, string>(s, $"Обнаружен командный блок {errType} находящийся в неправильном положении.");
        }
        Reset();
    }

    private Node Block()
    {
        Node query = new Node(BlockType.QUERY);
        Expect(BlockType.QUERY);
        List<Node> nodes = new List<Node>();
        nodes.Add(SelectNode());
        query._nodes = nodes;
        return query;
    }

    private Node SelectNode()
    {
        Expect(BlockType.SELECT);
        Node select = new Node(_prevToken._type, _prevToken._value);
        List<Node> nodes = new List<Node>();
        Expect(BlockType.FIELD_NAME);
        Node field = new Node(_prevToken._type, _prevToken._value);
        //nodes.Add(new Node(_prevToken._type, _prevToken._value));
        field._nodes = FieldNode();
        nodes.Add(field);
        Expect(BlockType.FROM);
        if (_prevToken._id != null)
        {
            nodes.Add(FromNode());
            if (_curToken._type == BlockType.WHERE)
            {
                nodes.Add(WhereNode());
            }
        }
        select._nodes = nodes;
        return select;
    }

    private List<Node> FieldNode()
    {
        List<Node> fields = new List<Node>();
        //NextToken();
        while (_curToken._type == BlockType.FIELD_NAME)
        {
            NextToken();
            fields.Add(new Node(_prevToken._type, _prevToken._value));
            //Expect(BlockType.FIELD_NAME);
        }
        return fields;
    }
    private Node FromNode()
    {
        Node from = new Node(_prevToken._type, _prevToken._value);
        List<Node> nodes = new List<Node>();
        Expect(BlockType.TABLE_NAME);
        Node table = new Node(_prevToken._type, _prevToken._value);
        List<Node> t_nodes = new List<Node>();
        while(_curToken._type == BlockType.TABLE_NAME)
        {
            NextToken();
            t_nodes.Add(new Node(_prevToken._type, _prevToken._value));
        }
        table._nodes = t_nodes;
        nodes.Add(table);
        from._nodes = nodes;
        return from;
    }

    private Node WhereNode()
    {
        NextToken();
        Node where = new Node(_prevToken._type, _prevToken._value);
        List<Node> nodes = new List<Node>();
        Expect(BlockType.CONDITION);
        nodes.Add(ConditionNode());
        where._nodes = nodes;
        return where;
    }
    private Node ConditionNode()
    {
        Node condition = new Node(_prevToken._type, _prevToken._value);
        List<Node> nodes = new List<Node>();
        if (_curToken._type == BlockType.SELECT)
        {
            nodes.Add(SelectNode());
        }
        condition._nodes = nodes;
        return condition;
    }
}

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

    public string toSql(string space = "")
    {
        string _space = space;
        string ret = "";
        if (_nodes != null && _nodes.Count != 0)
            foreach (Node node in _nodes)
            {
                ret += node.typeToString(ref _space) + node.toSql(_space);
            }

        return ret;
    }

    private string typeToString(ref string space)
    {
        string ret = "";
        switch (_type)
        {
            case BlockType.SELECT:
                ret += $"{space}SELECT ";
                break;
            case BlockType.FIELD_NAME:
                ret += $"{_type} {_value}";
                if (_nodes != null && _nodes.Count != 0) ret += ", ";
                break;
            case BlockType.FROM:
                ret += $"<br/>{space}FROM ";
                break;
            case BlockType.TABLE_NAME:
                ret += $"{_type} {_value}";
                if (_nodes != null && _nodes.Count != 0) ret += ", ";
                break;
            case BlockType.WHERE:
                ret += $"<br/>{space}WHERE ";
                break;
            case BlockType.CONDITION:
                ret += $"{_type} {_value}\n";
                if (_nodes != null && _nodes.Count != 0) space += "    ";
                break;

        }
        return ret;
    }
}

enum BlockType { ERROR = -1, QUERY = 0, SELECT = 1, FROM = 2, TABLE_NAME = 3, FIELD_NAME = 4, WHERE = 5, CONDITION = 6 }
class Constants {

    public static readonly Dictionary<string, int> Types = new Dictionary<string, int>()
    {
        {"QUERY", 0},
        {"SELECT", 1},
        {"FROM", 2},
        {"TABLE_NAME", 3},
        {"FIELD_NAME", 4},
        {"WHERE", 5},
        {"CONDITION", 6},
    };
}