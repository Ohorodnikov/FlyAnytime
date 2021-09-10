using FlyAnytime.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Controllers
{
    [ApiController]
    public class StartController : Controller
    {
        [Route("start")]
        public IActionResult Index()
        {
            return Json("started login");
        }

        [Route("init")]
        public IActionResult Init([FromServices] ICommonSettings settings)
        {
            var gatewayUrl = Request.Headers["GatewayUrl"].ToString();
            settings.ApiGatewayUrl = gatewayUrl;
            return Ok();
        }
    }
}
