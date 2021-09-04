using FlyAnytime.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Controllers
{
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
