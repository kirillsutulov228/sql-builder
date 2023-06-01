class SyntAnalyzer
{
    List<Lexem> _tokens = new List<Lexem>();
    Lexem _curToken, _prevToken;
    public Tuple<string, string> error = new Tuple<string, string>("", "");
    private bool _hasErrors = false;

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

    private bool ExpectFieldOrAgregate()
    {
        if(_curToken._type == BlockType.FIELD_NAME || isAgregateFunc())
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
        _hasErrors = true;
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
        if(!ExpectFieldOrAgregate()) return select;
        Node field = new Node(_prevToken._type, _prevToken._value);
        //nodes.Add(new Node(_prevToken._type, _prevToken._value));
        field._nodes = FieldNameNode();
        nodes.Add(field);
        if (_hasErrors) return select;
        if(Expect(BlockType.FROM))
        {
            nodes.Add(FromNode());
            if (_curToken._type == BlockType.JOIN)
            {
                nodes.Add(JoinNode());
            }
            if (_curToken._type == BlockType.WHERE)
            {
                nodes.Add(WhereNode());
            }
            if(_curToken._type == BlockType.GROUP_BY)
            {
                nodes.Add(GroupByNode());
            }
            if(_curToken._type == BlockType.ORDER_BY)
            {
                nodes.Add(OrderByNode());
            }
            
        }
        select._nodes = nodes;
        return select;
    }

    private List<Node> FieldNameNode()
    {
        List<Node> fields = new List<Node>();
        //NextToken();
        while (_curToken._id != null && _tokens.Count != 0)
        {
            if (isAgregateFunc())
            {
                NextToken();
                Node agregateFunc = new Node(_prevToken._type, _prevToken._value);
                if (!Expect(BlockType.FIELD_NAME)) return fields;
                agregateFunc._nodes = new List<Node>();
                agregateFunc._nodes.Add(new Node(_prevToken._type, _prevToken._value));
                fields.Add(agregateFunc);
            }
            else if (_curToken._type == BlockType.FIELD_NAME)
            {
                NextToken();
                fields.Add(new Node(_prevToken._type, _prevToken._value));
                //Expect(BlockType.FIELD_NAME);
            }
            else break;
        }
        return fields;
    }
    private Node FromNode()
    {
        Node from = new Node(_prevToken._type, _prevToken._value);
        List<Node> nodes = new List<Node>();
        if(!Expect(BlockType.TABLE_NAME)) return from;
        nodes.Add(TableNameNode());
        from._nodes = nodes;
        return from;
    }

    private Node TableNameNode()
    {
        Node table = new Node(_prevToken._type, _prevToken._value);
        List<Node> t_nodes = new List<Node>();
        while (_curToken._type == BlockType.TABLE_NAME)
        {
            NextToken();
            t_nodes.Add(new Node(_prevToken._type, _prevToken._value));
        }
        table._nodes = t_nodes;
        return table;
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
            if(_curToken._type == BlockType.AND || _curToken._type == BlockType.OR)
            {
                NextToken();
                nodes.Add(new Node(_prevToken._type, _prevToken._value));
                if (Expect(BlockType.CONDITION)) nodes.Add(ConditionNode());
            }
            else if(_curToken._type == BlockType.SELECT) nodes.Add(SelectNode());
        }
        condition._nodes = nodes;
        return condition;
    }

    private Node JoinNode()
    {
        NextToken();
        Node join = new Node(_prevToken._type, _prevToken._value);
        if (!Expect(BlockType.TABLE_NAME)) return join;
        //Node tableName = new Node(_prevToken._type, _prevToken._value);
        join._nodes = new List<Node>();
        join._nodes.Add(new Node(_prevToken._type, _prevToken._value));
        if (!Expect(BlockType.ON)) return join;
        Node on = new Node(_prevToken._type, _prevToken._value);
        if (!Expect(BlockType.CONDITION))
        {
            join._nodes.Add(on);
            return join;
        }
        on._nodes = new List<Node>();
        on._nodes.Add(new Node(_prevToken._type, _prevToken._value));
        join._nodes.Add(on);
        return join;
    }

    private Node GroupByNode()
    {
        NextToken();
        Node group = new Node(_prevToken._type, _prevToken._value);
        group._nodes = new List<Node>();
        if (!ExpectFieldOrAgregate()) return group;
        Node field = new Node(_prevToken._type, _prevToken._value);
        field._nodes = FieldNameNode();
        group._nodes.Add(field);
        if (_hasErrors) return group;
        if (_curToken._type == BlockType.HAVING)
        {
            NextToken();
            Node having = new Node(_prevToken._type, _prevToken._value);
            if (!Expect(BlockType.CONDITION)) return having;
            having._nodes = new List<Node>();
            having._nodes.Add(ConditionNode());
        }
        return group;
    }

    private Node OrderByNode()
    {
        NextToken();
        Node order = new Node(_prevToken._type, _prevToken._value);
        if (!ExpectFieldOrAgregate()) return order;
        Node field = new Node(_prevToken._type, _prevToken._value);
        order._nodes = new List<Node>();
        field._nodes = FieldNameNode();
        order._nodes.Add(field);
        if (_hasErrors) return order;
        if (_curToken._type == BlockType.ASC || _curToken._type == BlockType.DESC)
        {
            NextToken();
            order._nodes.Add(new Node(_prevToken._type, _prevToken._value));
        }
        return order;
    }

    private bool isAgregateFunc()
    {
        return _curToken._type == BlockType.COUNT || _curToken._type == BlockType.MAX || _curToken._type == BlockType.MIN ||
            _curToken._type == BlockType.SUM || _curToken._type == BlockType.AVG;
    }
}
