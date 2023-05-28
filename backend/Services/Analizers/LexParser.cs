class LexParser
{
    private List<Lexem> _lexems = new List<Lexem>();

    public LexParser() { }

    public List<Lexem> Parse(List<RawQueryNode> rawQueryNodes)
    {
        foreach (RawQueryNode rawQueryNode in rawQueryNodes)
        {
            getLexems(rawQueryNode);
        }
        return _lexems;
    }

    private void getLexems(RawQueryNode curNode)
    {
        if (curNode == null) return;
        if (curNode.BlockType != "QUERY" && curNode.ParentId == null)
        {
            _lexems.Add(new Lexem(BlockType.ERROR, curNode.Id));
            return;
        }
        //_blockId = curNode.Id;
        //_parentId = curNode.ParentId;
        Lexem lexem = new Lexem();
        lexem._type = findType(curNode.BlockType);
        lexem._id = curNode.Id;
        lexem._value = getProperty(curNode.Properties);
        _lexems.Add(lexem);
        if (curNode.Children == null) return;
        foreach (RawQueryNode node in curNode.Children)
        {
            getLexems(node);
        }
    }

    private Dictionary<string, string> getProperty(Dictionary<string,PropertyType>? properties)
    {
        Dictionary<string, string> _property = new Dictionary<string, string>();
        if (properties != null && properties.Count != 0)
        {
            foreach (KeyValuePair<string, PropertyType> prop in properties)
            {
                _property.Add(prop.Key, prop.Value.value);
            }
        }
        return _property;
    }

    private BlockType findType(string type)
    {
        var res = Constants.Types.ContainsKey(type);
        if (res) return (BlockType)Constants.Types[type];
        return BlockType.ERROR;
    }
}