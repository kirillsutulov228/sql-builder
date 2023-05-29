﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
public class CheckAnswerService
{
    private string _userQuery;
    private string _answerQuery;

    public CheckAnswerService() {}

    public TaskResultDto Check(int taskNum, string userQuery)
    {
        TaskResultDto taskResultDto = new TaskResultDto();
        using(TaskDbContext db = new TaskDbContext())
        {
            var tasksRes = db.Tasks.FromSqlRaw($"SELECT * FROM Tasks WHERE taskNum = {taskNum}").ToList();
            _answerQuery = tasksRes.First().correctAnswer;
        }

        string DbPath = @"DataBase\TaskDB.db";
        List<Dictionary<string, string>> answerRes = new List<Dictionary<string, string>>();
        using (SqliteConnection con = new SqliteConnection($"Data Source={DbPath}"))
        {
            con.Open();
            SqliteCommand cmd = con.CreateCommand();
            cmd.Connection = con;
            cmd.CommandText = userQuery;
            List<Dictionary<string, string>> userRes = SqlResultReader(cmd.ExecuteReader());
            cmd.CommandText = _answerQuery;
            answerRes = SqlResultReader(cmd.ExecuteReader());
            if (!CompareResults(userRes, answerRes))
            {
                taskResultDto.result = "К сожалению, результат запроса, который был составлен вами, оказался неверным." +
                     " Попробуйте составить инчае и отправить снова!";
                taskResultDto.isCorrect = false;
                return taskResultDto;
            }
        }
        TableView tableView = new TableView();
        
        taskResultDto.result = $"Отлично! Результат выполения собранного вами SQL запроса верный." +
            $"\nРеузльтат запроса выглядит следующим образом:\n{tableView.GetTable(answerRes)}";
        taskResultDto.isCorrect = true;
        return taskResultDto;
    }

    private bool CompareResults(List<Dictionary<string, string>> a, List<Dictionary<string, string>> b)
    {
        if (a.Count != b.Count) return false;
        for(int i = 0; i < a.Count(); i++)
        {
            if(a[i].Count != b[i].Count) return false;
            for(int j = 0; j < a[i].Count(); j++) if (!a[i].ElementAt(j).Value.Equals(b[i].ElementAt(j).Value)) return false;
        }
        return true;
    }

    private List<Dictionary<string, string>> SqlResultReader(SqliteDataReader reader)
    {
        List<Dictionary<string, string>> res = new List<Dictionary<string, string>>();
        //List<List<(string, string)>> res = new List<List<(string, string)>>();
        while (reader.Read())
        {
            Dictionary<string, string> resItem = new Dictionary<string, string>();
            string fieldName = "", fieldVal = "";
            for (int item = 0; item < reader.FieldCount; item++)
            {
                fieldName = reader.GetName(item).ToString();
                fieldVal = fieldVal == null ? "" : reader.GetValue(item).ToString();
                resItem.Add(fieldName, fieldVal);
            }
            res.Add(resItem);
        }
        reader.Close();
        return res;
    }
}