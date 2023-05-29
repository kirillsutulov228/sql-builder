using Microsoft.EntityFrameworkCore;

public class AvaibleTaskService
{
    public List<TaskInfo> GetAvaibleTasks()
    {
        List<TaskInfo> tasks = new List<TaskInfo>();
        using(TaskDbContext db = new TaskDbContext())
        {
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
            foreach (Task task in rawTasks)
                if (task.taskNum == taskNum)
                {
                    taskInfo = new TaskInfo { taskNum = task.taskNum, taskName = task.taskName, taskDescription = task.taskDescription };
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
