using Microsoft.AspNetCore.Mvc;

[Route("/api")]
public class SqlTranslatorController : Controller
{
    private SqlTranslatorService _sqlTranslatorService;
    private AvaibleTaskService _taskService;

    public SqlTranslatorController(SqlTranslatorService sqlTranslatorService)
    {
        _sqlTranslatorService = sqlTranslatorService;
    }

    [HttpPost("query/parse")]
    public IActionResult ParseQueryNode([FromBody] List<RawQueryNode> rawQueryNode)
    {
        return base.Ok(_sqlTranslatorService.Translate(rawQueryNode));
    }

    [HttpPost("query/table/{name}")]
    public IActionResult SqlQueryResult(String sqlQuery)
    {
        return base.Ok(sqlQuery);
    }

    [HttpPost("tasks")]
    public IActionResult GetTasks()
    {
        return base.Ok(_taskService.GetAvaibleTasks());
    }
}