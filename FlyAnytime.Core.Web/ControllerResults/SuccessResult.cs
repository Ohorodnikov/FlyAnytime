using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.Web.ControllerResults
{
    public class SuccessResult : BaseControllerResult
    {
        public SuccessResult(object data) : base(true, data) { }
    }
}
