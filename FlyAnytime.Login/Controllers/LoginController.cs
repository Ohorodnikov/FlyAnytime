﻿using FlyAnytime.Login.EF;
using FlyAnytime.Login.Helpers;
using FlyAnytime.Login.JWT;
using FlyAnytime.Login.Models;
using FlyAnytime.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.Login.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LoginController : Controller
    {
        LoginContext _dbContext;
        ITokenBuilder _tokenBuilder;
        IOclHelper _oclHelper;
        public LoginController(LoginContext dbContext, ITokenBuilder tokenBuilder, IOclHelper oclHelper)
        {
            _dbContext = dbContext;
            _tokenBuilder = tokenBuilder;
            _oclHelper = oclHelper;
        }

        private class SuccessDataModel
        {
            public SuccessDataModel(bool success, object data = null)
            {
                Success = success;
                Data = data;
            }
            public bool Success { get; set; }
            public object Data { get; set; }
        }

        [Authorize]
        public async Task<IActionResult> CheckJwtToken()
        {
            return Json(new { jwtStatus = true });
            return Ok();
        }

        public async Task<IActionResult> GenerateOclLink(long userId)
        {
            var ocl = await _oclHelper.Create(userId);

            if (ocl == null)
                return Json(new SuccessDataModel(false));

            return Json(new SuccessDataModel(true, ocl));
        }

        [Route("/jwtocl/{userLogin}")]
        public async Task<IActionResult> GetJwtForOneClickLogin(string userLogin)
        {
            var userLoginInfo = await _oclHelper.FindOneClickLoginByUrl(userLogin);

            if (userLoginInfo == null)
                return Json("Not valid url");

            var now = DateTimeHelper.UnixNow;

            if (userLoginInfo.ExpireDateTime <= now)
                return Json("Url expired. Generate new login url");

            var jwt = await _tokenBuilder.BuildToken(userLoginInfo.User.Id);

            return Ok(jwt);
        }
    }
}
