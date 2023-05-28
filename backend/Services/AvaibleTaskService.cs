using Microsoft.EntityFrameworkCore;

public class AvaibleTaskService
{
    public List<TaskInfo> GetAvaibleTasks()
    {
        List<TaskInfo> tasks = new List<TaskInfo>();
        using(TaskDbContext db = new TaskDbContext())
        {
            //Task task3 = new Task
            //{
            //    taskNum = 3,
            //    taskName = "Первые условия",
            //    taskDescription = "Вам необходимо написать запрос SELECT так, чтобы " +
            //    "в результате выполения запроса были выведены значения полей userId, firstname и age только тех пользователей, чем возраст достиг 18, из таблицы Users.",
            //    correctAnswer = "SELECT userId, firstname, age FROM Users WHERE age > 17"
            //};

            //db.Tasks.AddRange(task1, task2, task3);
            //db.SaveChanges();

            var rawTasks = db.Tasks.FromSqlRaw("SELECT * FROM Tasks").ToList();
            tasks = convertRawTasks(rawTasks);

        }
        return tasks;
    }

    public TaskInfo GetTaskByNum(int taskNum)
    {
        TaskInfo taskInfo = new TaskInfo();
        using (TaskDbContext db = new TaskDbContext())
        {
            var rawTasks = db.Tasks.FromSqlRaw("SELECT * FROM Tasks").ToList();
            foreach(Task task in rawTasks)
                if(task.taskNum == taskNum)
                {
                    taskInfo = new TaskInfo { taskNum = task.taskNum, taskName = task.taskName, taskDescription = task.taskDescription};
                    break;
                }

        }
        return taskInfo;
    }

    private List<TaskInfo> convertRawTasks(List<Task> rawTasks)
    {
        List<TaskInfo> tasks = new List<TaskInfo>();
        foreach(Task rawTask in rawTasks)
        {
            tasks.Add(new TaskInfo() { taskNum = rawTask.taskNum, taskName = rawTask.taskName, taskDescription = rawTask.taskDescription });
        }
        return tasks;
    }
}
