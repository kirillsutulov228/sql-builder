using Microsoft.AspNetCore.Mvc;

[Route("/api")]
public class ExampleController : Controller
{
  private ExampleService _exampleService;
  
  public ExampleController(ExampleService exampleService)
  {
    _exampleService = exampleService;
  }

  [HttpGet("hello")]
  public IActionResult Hello()
  {
    return base.Ok(_exampleService.SayHello());
  }

  [HttpGet("hello/{id}")]
  public IActionResult Hello(int id)
  {
    return base.Ok(new {
      message = $"Hello, {id}"
    });
  }

  [HttpPost("query/parse")]
  public IActionResult ParseQueryNode([FromBody] List<RawQueryNode> rawQueryNode)
  {
    return base.Ok(rawQueryNode);
  }
}
