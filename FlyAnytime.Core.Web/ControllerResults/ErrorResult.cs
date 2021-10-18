using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.Web.ControllerResults
{
    public class ErrorResult : BaseControllerResult
    {
        public ErrorResult(object data) : base(false, data) { }
    }
}
