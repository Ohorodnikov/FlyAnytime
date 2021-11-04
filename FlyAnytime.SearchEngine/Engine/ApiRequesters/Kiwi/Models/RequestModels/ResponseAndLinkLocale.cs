namespace FlyAnytime.SearchEngine.Engine.ApiRequesters.Kiwi.Models.RequestModels
{
    /// <summary>
    /// the language of city names in the response and also language of kiwi.com website to which deep_links lead
    /// </summary>
    public class ResponseAndLinkLocale : BaseStringParam
    {
        public ResponseAndLinkLocale(string value) : base("locale", value)
        {
        }
    }
}
