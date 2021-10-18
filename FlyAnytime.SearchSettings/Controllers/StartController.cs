using FlyAnytime.Core;
using FlyAnytime.Core.Web;
using FlyAnytime.SearchSettings.MongoDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Controllers
{
    [ApiController]
    public class StartController : BaseInitController<IMongoDbContext>
    {
        //[Route("start")]
        //public IActionResult Index()
        //{
        //    return Json("started login");
        //}

        //[Route("init")]
        //public IActionResult Init([FromServices] ICommonSettings settings)
        //{
        //    var gatewayUrl = Request.Headers["GatewayUrl"].ToString();
        //    settings.ApiGatewayUrl = gatewayUrl;
        //    return Ok();
        //}

        //[Route("ReCreateDb")]
        //public async Task<IActionResult> ResetDb([FromServices] IMongoDbContext dbContext)
        //{
        //    var secretKey = Request.Headers["resetDbSecretKey"].ToString();
        //    var referer = Request.Headers["Referer"].ToString();
        //    if (secretKey == "19D0DE6E-77A0-4F0D-A9AC-65AEA1470BCB" && referer == "SearchSettings.Test")
        //    {
        //        await dbContext.InitDatabase();
        //    }
        //    return Ok();
        //}
    }
}
