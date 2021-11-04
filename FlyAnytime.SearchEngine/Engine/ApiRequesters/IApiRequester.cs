using FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchEngine.Engine.ApiRequesters
{
    public interface IApiRequester : IApiRequestCreator
    {
        IReadOnlyCollection<IApiRequestSender> Requests { get; }
    }

    public interface IApiRequestCreator
    {
        IRequestDescriptor CreateRequest(string key);
    }

    public interface IRequestDescriptor
    {
        string Key { get; }
        IRequestDescriptor AddParam<TValue>(IBaseSearchParam<TValue> searchParam);
        IApiRequestSender Build();
    }

    public interface IApiRequestSender
    {
        string Key { get; }
        Task<List<ApiResultModel>> Send();
    }

    public static class ApiRequestHelper
    {
        public static string GenerateRequestGroupName(string cityFrom, string cityTo)
        {
            return $"{cityFrom}-{cityTo}";
        }
    }
}
