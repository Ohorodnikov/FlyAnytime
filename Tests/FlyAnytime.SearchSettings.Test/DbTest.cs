using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlyAnytime.SearchSettings.Test
{
    public class DbTest : TestBase
    {
        public override string ControllerName => "";

        public async Task ReCreateDb()
        {
            var key = "19D0DE6E-77A0-4F0D-A9AC-65AEA1470BCB";

            var msg = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
            };

            msg.Headers.Add("resetDbSecretKey", key);
            msg.Headers.Add("Referer", "SearchSettings.Test");

            await Send(msg, "ResetDb");
        }
    }
}
