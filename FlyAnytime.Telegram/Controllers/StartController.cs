using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Controllers
{
    public class StartController : Controller
    {
        [Route("start")]
        public IActionResult Index()
        {
            return Json("started");
        }
    }
}
