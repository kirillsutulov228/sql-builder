using Microsoft.AspNetCore.Mvc;

[Route("/api")]
public class SqlTranslatorController : Controller
{
    private SqlTranslatorService _sqlTranslatorService;

    public SqlTranslatorController(SqlTranslatorService sqlTranslatorService)
    {
        _sqlTranslatorService = sqlTranslatorService;
    }

    [HttpPost("query/parse")]
    public IActionResult ParseQueryNode([FromBody] List<RawQueryNode> rawQueryNode)
    {

        return base.Ok(_sqlTranslatorService.Translate(rawQueryNode));
    }


}