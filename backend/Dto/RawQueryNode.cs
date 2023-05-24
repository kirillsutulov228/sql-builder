public class RawQueryNode
{
    public string Id { get; set; }
    public string BlockType { get; set; }
    public string ParentId { get; set; }
    public List<RawQueryNode>? Children { get; set; }
    public Dictionary<string, PropertyType>? Properties { get; set; }
}

public class PropertyType
{
    public string title { get; set; }
    public string value { get; set; }

}