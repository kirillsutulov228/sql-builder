using Microsoft.EntityFrameworkCore;

public class CheckAnswerService
{
    private string _userQuery;
    private string _answerQuery;

    public CheckAnswerService() {}

    public string Check(int taskNum, string userQuery)
    {
        using (TaskDbContext db = new TaskDbContext())
        {
            var taskQuery = db.Tasks.FromSqlRaw("SELECT * FROM Tasks").Where(x => x.taskNum == taskNum).ToList();
            foreach (var query in taskQuery)
                _answerQuery = query.correctAnswer;

            var userQueryRes = db.Users.FromSql($"{userQuery}").ToList();
        }
        return _answerQuery;
    }
}