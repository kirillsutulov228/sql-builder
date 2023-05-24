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
        _curToken = new Lexem(BlockType.ERROR, "-1");
    }
    private bool Expect(BlockType type)
    {
        if (_curToken._type == type)
        {
            NextToken();
            return true;
        }
        else
        {
            if (_curToken._id == null)
            {
                _prevToken = _curToken;
                Error("-1");
            }
            else Error(_curToken._id);
            return false;
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
        if (Expect(BlockType.QUERY))
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(SelectNode());
            query._nodes = nodes;
        }
        return query;
    }

    private Node SelectNode()
    {
        if(!Expect(BlockType.SELECT)) return new Node();
        Node select = new Node(_prevToken._type, _prevToken._value);
        List<Node> nodes = new List<Node>();
        if(!Expect(BlockType.FIELD_NAME)) return select;
        Node field = new Node(_prevToken._type, _prevToken._value);
        //nodes.Add(new Node(_prevToken._type, _prevToken._value));
        field._nodes = FieldNode();
        nodes.Add(field);
        if(Expect(BlockType.FROM))
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
        if(!Expect(BlockType.TABLE_NAME)) return from;
        Node table = new Node(_prevToken._type, _prevToken._value);
        List<Node> t_nodes = new List<Node>();
        while (_curToken._type == BlockType.TABLE_NAME)
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
        if(!Expect(BlockType.CONDITION)) return where;
        //if (_curToken._id == null && _prevToken._type != BlockType.CONDITION) return where;
        nodes.Add(ConditionNode());
        where._nodes = nodes;
        return where;
    }
    private Node ConditionNode()
    {
        Node condition = new Node(_prevToken._type, _prevToken._value);
        List<Node> nodes = new List<Node>();
        if ((_tokens.Count != 0 && _tokens != null) || _curToken._type != BlockType.ERROR)
        {
            nodes.Add(SelectNode());
        }
        condition._nodes = nodes;
        return condition;
    }
}
