using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models
{
    public interface IBaseSearchParam
    {
        string ParamName { get; }
        string ToSearchString();
    }

    public interface IBaseSearchParam<TValue> : IBaseSearchParam
    {
        TValue ParamValue { get; }
    }

    public abstract class BaseSearchParam<TValue> : IBaseSearchParam<TValue>
    {
        public BaseSearchParam(string paramName, TValue value)
        {
            ParamName = paramName;
            ParamValue = value;
        }

        public string ParamName { get; }
        public TValue ParamValue { get; }

        public abstract string ToSearchString();
    }
}
