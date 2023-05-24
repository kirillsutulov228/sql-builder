class Lexem
{
    public BlockType _type { get; set; }
    public string _id { get; set; }
    public Dictionary<string, string>? _value { get; set; }

    public Lexem(BlockType type, string id, Dictionary<string, string>? value = null)
    {
        _type = type;
        _id = id;
        _value = value;
    }

    public Lexem() { }
}