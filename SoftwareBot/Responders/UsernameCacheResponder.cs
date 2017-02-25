using MargieBot;
using System;
using System.Collections.Generic;

namespace SoftwareBot
{
    public class UsernameCacheResponder : SBResponder
    {
        public IReadOnlyDictionary<string, string> userNameCache = new Dictionary<string, string>();
        public override bool CanRespond(ResponseContext context)
        {
            userNameCache = context.UserNameCache;
            return false;
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage { Text = String.Empty };
        }

        public override string ToString()
        {
            return "Username Cache";
        }
    }
}
