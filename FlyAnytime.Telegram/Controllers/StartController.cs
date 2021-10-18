using FlyAnytime.Core;
using FlyAnytime.Core.Web;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Telegram.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Telegram.Controllers
{
    public class StartController : BaseInitController<TelegramContext>
    {
        //[Route("start")]
        //public IActionResult Index()
        //{
        //    return Json("started tg");
        //}

        //[Route("init")]
        //public async Task<IActionResult> Init([FromServices] ICommonSettings settings, [FromServices] TgWebhook tgWebhook)
        //{
        //    var gatewayUrl = Request.Headers["GatewayUrl"].ToString();
        //    settings.ApiGatewayUrl = gatewayUrl;

        //    await tgWebhook.StartAsync();

        //    return Ok();
        //}
    }
}
