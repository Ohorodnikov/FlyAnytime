using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Messaging.Messages.SearchSettings
{
    public class DeleteCityMessage : BaseMessage
    {
        public DeleteCityMessage(string code)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
