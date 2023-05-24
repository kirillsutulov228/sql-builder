class LexParser
{
    private string _blockId = "";
    private List<Lexem> _lexems = new List<Lexem>();
    private string _parentId = "";

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
        if (res) return (BlockType)Constants.Types[type];
        return BlockType.ERROR;
    }
}