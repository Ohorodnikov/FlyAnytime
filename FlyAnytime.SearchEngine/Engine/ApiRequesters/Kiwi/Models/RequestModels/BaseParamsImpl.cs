using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    public class BaseNumericParam : BaseSearchParam<long>
    {
        public BaseNumericParam(string paramName, long value) : base(paramName, value) { }

        public override string ToSearchString() => ParamValue.ToString();
    }

    public class BaseStringParam : BaseSearchParam<string>
    {
        public BaseStringParam(string paramName, string value) : base(paramName, value) { }

        public override string ToSearchString() => ParamValue;
    }

    public class BaseDateTimeParam : BaseSearchParam<DateTime>
    {
        public BaseDateTimeParam(string paramName, DateTime value) : base(paramName, value) { }

        public override string ToSearchString() => ParamValue.ToString("dd'/'MM'/'yyyy");
    }
}
