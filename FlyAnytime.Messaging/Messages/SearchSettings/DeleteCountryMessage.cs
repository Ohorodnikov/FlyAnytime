using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchSettings
{
    public class DeleteCountryMessage : BaseMessage
    {
        public DeleteCountryMessage(string code)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
