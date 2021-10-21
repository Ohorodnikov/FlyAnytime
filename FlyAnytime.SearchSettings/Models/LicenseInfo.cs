using FlyAnytime.Core.Enums;
using FlyAnytime.SearchSettings.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Models
{
    public class LicenseInfo : MongoInternalEntity
    {
        public LicenseType LicenseType { get; set; }
        public long ExpiresOn { get; set; }
    }
}
