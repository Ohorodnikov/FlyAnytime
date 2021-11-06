using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTests.Models
{
    public interface IBaseControllerModel
    {
        string MicroserviceAlias { get; }
        string ControllerName { get; }
    }

    public interface ILocalizationEntity
    {
        string Code { get; set; }
        string Name { get; set; }
        List<Localization> Localizations { get; set; }
    }
}
