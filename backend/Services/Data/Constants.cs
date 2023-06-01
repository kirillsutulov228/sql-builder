﻿enum BlockType { 
    ERROR = -1,
    QUERY = 0,
    SELECT = 1,
    FROM = 2,
    TABLE_NAME = 3,
    FIELD_NAME = 4,
    WHERE = 5,
    CONDITION = 6,
    AND = 7,
    OR = 8,
    JOIN = 9,
    ORDER_BY = 10,
    GROUP_BY = 11,
    ON = 12,
    COUNT = 13,
    MAX = 14,
    MIN = 15,
    SUM = 16,
    AVG = 17,
    HAVING = 18,
    ASC = 19,
    DESC = 20,
}
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
        {"JOIN", 9},
        {"ORDER_BY", 10},
        {"GROUP_BY", 11},
        {"ON", 12},
        {"COUNT", 13},
        {"MAX", 14},
        {"MIN", 15},
        {"SUM", 16},
        {"AVG", 17},
        {"HAVING", 18},
        {"ASC", 19},
        {"DESC", 20},
    };
}