using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class SqlTranslatorService
{
    public Tuple<String, ErrorNode> Translate(List<RawQueryNode> rawQueryNodes)
    {
        LexParser parser = new LexParser();
        List<Lexem> tokens = parser.Parse(rawQueryNodes);
        SyntAnalyzer syntAnalyzer = new SyntAnalyzer();
        Node syntTree = syntAnalyzer.Analyze(tokens);
        Console.WriteLine(syntTree.toSql());
        Console.WriteLine(syntAnalyzer.error.Item2);
        ErrorNode error = new ErrorNode();
        error.blockId = syntAnalyzer.error.Item1;
        error.message = syntAnalyzer.error.Item2;

        //string query = "SELECT * FROM Users";

        //CheckAnswerService checkAnswerService = new CheckAnswerService();
        //Console.WriteLine(checkAnswerService.Check(1, query).result);

        return new Tuple<string, ErrorNode>(syntTree.toSql(), error);
    }
}