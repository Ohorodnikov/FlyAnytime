using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTests.Models
{
    public class Language : IBaseControllerModel
    {
        public string MicroserviceAlias => "ss";
        public string ControllerName => "Language";

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
