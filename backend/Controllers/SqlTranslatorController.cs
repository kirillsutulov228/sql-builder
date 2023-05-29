using Microsoft.AspNetCore.Mvc;

[Route("/api")]
public class SqlTranslatorController : Controller
{
    private SqlTranslatorService _sqlTranslatorService;
    private AvaibleTaskService _taskService;
    private CheckAnswerService _checkAnswerService;

    public SqlTranslatorController(SqlTranslatorService sqlTranslatorService, AvaibleTaskService taskService)
    {
        _sqlTranslatorService = sqlTranslatorService;
        _taskService = taskService;
    }

    [HttpPost("query/parse")]
    public IActionResult ParseQueryNode([FromBody] List<RawQueryNode> rawQueryNode)
    {
        return base.Ok(_sqlTranslatorService.Translate(rawQueryNode));
    }

    [HttpPost("task/{taskNumber}")]
    public IActionResult CheckAnswer(int taskNumber, [FromBody] string query)
    {
        return base.Ok(_checkAnswerService.Check(taskNumber, query));
    }

    [HttpGet("tasks/{taskNum}")]
    public IActionResult GetTaskByNum(int taskNum)
    {
        return base.Ok(_taskService.GetTaskByNum(taskNum));
    }

    [HttpGet("tasks")]
    public IActionResult GetTasks()
    {
        return base.Ok(_taskService.GetAvaibleTasks());
    }
}