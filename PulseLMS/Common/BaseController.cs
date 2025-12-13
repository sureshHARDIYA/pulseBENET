using Microsoft.AspNetCore.Mvc;

namespace PulseLMS.Common;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController : Controller
{
}