using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchSettings
{
    public class DeleteAirportMessage : BaseMessage
    {
        public DeleteAirportMessage(string code)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
