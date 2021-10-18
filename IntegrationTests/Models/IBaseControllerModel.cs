using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTests.Models
{
    interface IBaseControllerModel
    {
        string MicroserviceAlias { get; }
        string ControllerName { get; }
    }
}
