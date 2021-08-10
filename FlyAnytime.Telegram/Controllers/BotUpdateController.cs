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
        //BotClient _client;
        //IServiceProvider _serviceProvider;
        //public BotUpdateController(ITelegramBotClient bot, IServiceProvider serviceProvider)
        //{
        //    //_serviceProvider = serviceProvider;
        //    //_client = new BotClient(bot);
        //}

        //[Route("/bot")]
        [HttpPost]
        public async Task<IActionResult> GetBotUpdate([FromServices] BotClient client, [FromBody] Update update)
        {
            await client.ProcessUpdate(update);
            return Ok();
        }
    }
}
