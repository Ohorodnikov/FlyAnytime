using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.Web.ControllerResults
{
    public class BaseControllerResult
    {
        public BaseControllerResult(bool success, object data)
        {
            Success = success;
            Data = data;
        }

        public bool Success { get; set; }
        public object Data { get; set; }
    }
}
