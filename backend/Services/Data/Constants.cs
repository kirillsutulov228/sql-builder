enum BlockType { ERROR = -1, QUERY = 0, SELECT = 1, FROM = 2, TABLE_NAME = 3, FIELD_NAME = 4, WHERE = 5, CONDITION = 6, AND = 7, OR = 8 }
class Constants
{

    public static readonly Dictionary<string, int> Types = new Dictionary<string, int>()
    {
        {"QUERY", 0},
        {"SELECT", 1},
        {"FROM", 2},
        {"TABLE_NAME", 3},
        {"FIELD_NAME", 4},
        {"WHERE", 5},
        {"CONDITION", 6},
        {"AND", 7},
        {"OR", 8},
    };
}