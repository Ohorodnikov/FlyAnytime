using System.Collections.Generic;

namespace IntegrationTests.Models
{
    public class Airport : IBaseControllerModel
    {
        public string MicroserviceAlias => "ss";

        public string ControllerName => "airport";

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CityId { get; set; }

        public IEnumerable<Localization> Localizations { get; set; }
    }
}
