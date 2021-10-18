using FlyAnytime.Core.Web.ControllerResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.Core.Web
{
    public class BaseFAController : Controller
    {
        protected IActionResult SuccessResult<TData>(TData data)
        {
            return Json(new SuccessResult(data));
        }

        protected IActionResult SuccessResult()
        {
            return Json(new SuccessResult(null));
        }

        protected IActionResult ErrorResult<TData>(TData data)
        {
            return Json(new ErrorResult(data));
        }
    }
}
