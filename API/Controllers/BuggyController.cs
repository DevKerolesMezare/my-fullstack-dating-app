using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuggyController : ControllerBase
{
    [HttpGet("auth")]
    public IActionResult GetAuth() => Unauthorized();

    [HttpGet("not-found")]
    public IActionResult GetNotFound() => NotFound();

    [HttpGet("server-error")]
    public IActionResult GetServerError() => throw new Exception("This is a server error");


    [HttpGet("bad-request")]
    public IActionResult GetBadRequest() => BadRequest("This was not a good request");



}
