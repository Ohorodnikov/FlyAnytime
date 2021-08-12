using FlyAnytime.Telegram.Bot;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FlyAnytime.Telegram.Controllers
{
    public class BotUpdateController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GetBotUpdate([FromServices] BotClient client, [FromBody] Update update)
        {
            await client.ProcessUpdate(update);
            return Ok();
        }
    }
}
