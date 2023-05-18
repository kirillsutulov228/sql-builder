public class RawQueryNode
{
  public string Id { get; set; }
  public string BlockType { get; set; }
  public List<RawQueryNode>? Children { get; set; }
  public Dictionary<String, Object>? Properties { get; set; }
}
