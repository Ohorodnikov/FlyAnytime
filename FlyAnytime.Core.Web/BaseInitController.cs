using FlyAnytime.Core.EfContextBase;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Core.Web
{
    public class BaseInitController<TContext> : Controller
        where TContext : IDbContextBase
    {
        [Route("start")]
        public virtual IActionResult Index()
        {
            return Json($"started {GetType().FullName}");
        }

        [Route("init")]
        public virtual IActionResult Init([FromServices] ICommonSettings settings)
        {
            var gatewayUrl = Request.Headers["GatewayUrl"].ToString();
            settings.ApiGatewayUrl = gatewayUrl;
            return Ok();
        }

        [Route("ReCreateDb")]
        [HttpPost]
        public virtual async Task<IActionResult> ReCreateDb([FromServices] TContext context)
        {
            var secretKey = Request.Headers["resetDbSecretKey"].ToString();
            var referer = Request.Headers["Referer"].ToString();
            if (secretKey == "19D0DE6E-77A0-4F0D-A9AC-65AEA1470BCB" && referer == "SearchSettings.Test")
            {
                await context.ReCreateDb();
            }
            return Ok();
        }
    }
}
