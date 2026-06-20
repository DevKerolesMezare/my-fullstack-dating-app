using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Entities;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
}
