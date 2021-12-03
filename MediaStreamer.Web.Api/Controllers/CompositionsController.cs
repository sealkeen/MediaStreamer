using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediaStreamer.Domain;
using MediaStreamer.WebApplication.Services;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Tickets.Web.Api.Controllers;

namespace MediaStreamer.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompositionsController : BaseController
    {
        private readonly CompositionStoreService _compositionsService;
        private readonly IWebHostEnvironment _env;
        public CompositionsController(IWebHostEnvironment env, CompositionStoreService compositionStoreService)
        {
            _env = env;
            _compositionsService = compositionStoreService;
        }

        [HttpGet]
        public ActionResult<List<Composition>> GetCompositions([FromServices] IWebHostEnvironment env)
        {
            var count = _compositionsService.GetCompositions().Count();
            return _compositionsService.GetCompositions().ToList();
        }

        [HttpPost("/api/[action]")]
        public async Task<ActionResult> RegisterComposition([FromBody] string JSONcomposition)
        {
            try
            {
                await _compositionsService.AddCompositionAsync(JSONcomposition);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
