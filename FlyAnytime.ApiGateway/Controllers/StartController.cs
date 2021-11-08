using FlyAnytime.Messaging;
using FlyAnytime.Messaging.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlyAnytime.ApiGateway.Controllers
{
    public class StartController : Controller
    {
        IConfiguration _configuration;
        IMessageBus _messageBus;
        public StartController(IConfiguration configuration, IMessageBus messageBus)
        {
            _configuration = configuration;
            _messageBus = messageBus;
        }

        [Route("start")]
        public async Task<IActionResult> Start()
        {
            System.Threading.Thread.Sleep(2000);
            var selfLocal = _configuration.GetSection("SelfUrl").Value;

            var msg = new AppInitMessage(selfLocal);

            _messageBus.Publish(msg);

            var res = new GatewayResultModel
            {
                Success = true,
                Content = "Started"
            };

            return Json(res);
        }

        [HttpPut]
        [Route("ReCreateAllDb")]
        public async Task<IActionResult> ReCreateAllDb()
        {
            var secretKey = Request.Headers["resetDbSecretKey"].ToString();
            var referer = Request.Headers["Referer"].ToString();

            if (secretKey != "19D0DE6E-77A0-4F0D-A9AC-65AEA1470BCB" || referer != "SearchSettings.Test")
            {
                return NotFound();
            }

            var msg = new ReCreateDbMessage();
            _messageBus.Publish(msg);

            var res = new GatewayResultModel
            {
                Success = true,
                Content = "Message was sent"
            };

            return Json(res);
        }

        [HttpPut]
        [Route("DeleteAllUsersData")]
        public async Task<IActionResult> DeleteAllUsersData()
        {
            var secretKey = Request.Headers["resetDbSecretKey"].ToString();
            var referer = Request.Headers["Referer"].ToString();

            if (secretKey != "19D0DE6E-77A0-4F0D-A9AC-65AEA1470BCB" || referer != "SearchSettings.Test")
            {
                return NotFound();
            }

            var msg = new DeleteAllUsersDataMessage();
            _messageBus.Publish(msg);

            var res = new GatewayResultModel
            {
                Success = true,
                Content = "Message was sent"
            };

            return Json(res);
        }
    }
}
