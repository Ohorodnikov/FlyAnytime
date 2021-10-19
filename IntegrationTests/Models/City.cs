using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTests.Models
{
    public class City : IBaseControllerModel
    {
        public string MicroserviceAlias => "ss";

        public string ControllerName => "city";

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; }

        public IEnumerable<Localization> Localizations { get; set; }
    }
}
