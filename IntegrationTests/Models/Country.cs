﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTests.Models
{
    public class Country : IBaseControllerModel, ILocalizationEntity
    {
        public string MicroserviceAlias => "ss";

        public string ControllerName => "Country";

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public string DefSearchCurrencyCode { get; set; }

        public List<Localization> Localizations { get; set; }
    }
}
