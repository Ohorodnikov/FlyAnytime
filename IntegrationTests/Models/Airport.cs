using System.Collections.Generic;

namespace IntegrationTests.Models
{
    public class Airport : IBaseControllerModel, ILocalizationEntity
    {
        public string MicroserviceAlias => "ss";

        public string ControllerName => "airport";

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityId { get; set; }

        public List<Localization> Localizations { get; set; }
    }
}
