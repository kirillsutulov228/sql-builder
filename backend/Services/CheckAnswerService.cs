using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
public class CheckAnswerService
{
    private string _userQuery;
    private string _answerQuery;

    public CheckAnswerService() {}

    public string Check(int taskNum, string userQuery)
    {
        using(TaskDbContext db = new TaskDbContext())
        {
            var tasksRes = db.Tasks.FromSqlRaw($"SELECT * FROM Tasks WHERE taskNum = {taskNum}").ToList();
            _answerQuery = tasksRes.First().correctAnswer;
        }

        string DbPath = @"DataBase\TaskDB.db";
        List<List<(string, string)>> answerRes = new List<List<(string, string)>>();
        using (SqliteConnection con = new SqliteConnection($"Data Source={DbPath}"))
        {
            con.Open();
            SqliteCommand cmd = con.CreateCommand();
            cmd.Connection = con;
            cmd.CommandText = userQuery;
            List<List<(string, string)>> userRes = SqlResultReader(cmd.ExecuteReader());
            cmd.CommandText = _answerQuery;
            answerRes = SqlResultReader(cmd.ExecuteReader());
            if(!CompareResults(userRes, answerRes)) return "Результат ";
        }
        TableView tableView = new TableView();
        
        return $"Отлично! Результат выполения собранного вами SQL запроса верный." +
            $"\nРеузльтат запроса выглядит следующим образом:\n{tableView.GetTable(answerRes)}";
    }

    private bool CompareResults(List<List<(string, string)>> a, List<List<(string, string)>> b)
    {
        if (a.Count != b.Count) return false;
        for(int i = 0; i < a.Count(); i++)
        {
            if(a[i].Count != b[i].Count) return false;
            for(int j = 0; j < a[i].Count(); j++) if (!a[i][j].Item2.Equals(b[i][j].Item2)) return false;
        }
        return true;
    }

    private List<List<(string, string)>> SqlResultReader(SqliteDataReader reader)
    {
        //List<Dictionary<string, string>> res = new List<Dictionary<string, string>>();
        List<List<(string, string)>> res = new List<List<(string, string)>>();
        List<(string, string)> resItem = new List<(string, string)>();
        while (reader.Read())
        {
            string fieldName = "", fieldVal = "";
            for (int item = 0; item < reader.FieldCount; item++)
            {
                fieldName = reader.GetName(item).ToString();
                fieldVal = fieldVal == null ? "" : reader.GetValue(item).ToString();
                resItem.Add((fieldName, fieldVal));
            }
            res.Add(resItem);
            resItem.Clear();
        }
        reader.Close();
        return res;
    }
}