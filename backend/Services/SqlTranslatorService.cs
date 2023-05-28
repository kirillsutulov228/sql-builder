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

        using (TaskDbContext taskDb = new TaskDbContext())
        {
            string first = Console.ReadLine();
            string last = Console.ReadLine();
            var user = new User
            {
                firstname = first,
                lastname = last,
                registrationDate = new DateOnly(2023, 5, 30)
            };

            taskDb.Add(user);
            taskDb.SaveChanges();
        }

        return new Tuple<string, ErrorNode>(syntTree.toSql(), error);
    }
}