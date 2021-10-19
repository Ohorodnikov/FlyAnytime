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
        TgWebhook _tgWebhook;
        public StartController(TgWebhook tgWebhook)
        {
            _tgWebhook = tgWebhook;
        }

        [Route("init")]
        public override async Task<IActionResult> Init([FromServices] ICommonSettings settings)
        {
            await base.Init(settings);

            await _tgWebhook.StartAsync();

            return Ok();
        }
    }
}
