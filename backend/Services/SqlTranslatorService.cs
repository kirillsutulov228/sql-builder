public class SqlTranslatorService
{
    public List<RawQueryNode> Translate(List<RawQueryNode> rawQueryNodes)
    {
        LexParser parser = new LexParser();
        List<Lexem> tokens = parser.Parse(rawQueryNodes);
        SyntAnalyzer syntAnalyzer = new SyntAnalyzer();
        Node syntTree = syntAnalyzer.Analyze(tokens);
        return rawQueryNodes;
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
        _curToken = new Lexem();
    }
    private void Expect(BlockType type)
    {
        if (_curToken._type == type)
        {
            NextToken();
            return;
        }
        else Error(_curToken._id.ToString());

    }

    private void Error(string s)
    {
        //throw new Exception($"Error: {s}");
        return;
    }

    private Node Block()
    {
        Expect(BlockType.QUERY);
        Node query = SelectNode();
        return query;
    }

    private Node SelectNode()
    {
        Expect(BlockType.SELECT);
        Node select = new Node(_prevToken._type, _prevToken._value);
        List<Node> nodes = new List<Node>();
        Expect(BlockType.FIELD_NAME);
        nodes.Add(new Node(_prevToken._type, _prevToken._value));
        Expect(BlockType.FROM);
        nodes.Add(FromNode());
        if(_curToken._type == BlockType.WHERE)
        {
            nodes.Add(WhereNode());
        }
        select._nodes = nodes;
        return select;
    }

    private Node FromNode()
    {
        Node from = new Node(_prevToken._type, _prevToken._value);
        List<Node> nodes = new List<Node>();
        Expect(BlockType.TABLE_NAME);
        nodes.Add(new Node(_prevToken._type, _prevToken._value));
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
    public string? _value { get; set; }

    public List<Node>? _nodes { get; set; }

    public Node(BlockType type, string? value)
    {
        _type = type;
        _value = value;
    }

    public Node(List<Node> nodes)
    {
        _nodes = nodes;
    }

    public string toSql()
    {
        return "";
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