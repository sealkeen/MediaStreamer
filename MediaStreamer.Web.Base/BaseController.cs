using Microsoft.AspNetCore.Mvc;

namespace Tickets.Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
    }
}
