using MargieBot;
using System;
using System.Collections.Generic;

namespace SoftwareBot
{
    public class UsernameCacheResponder : SBResponder
    {
        public Dictionary<string, string> userNameCache = new Dictionary<string, string>();

        public UsernameCacheResponder()
        {
            userNameCache.Add("B2HJYASGJ", "a web hook thing, I think");
            userNameCache.Add("B371F3CBB", "rollout");
            
        }
        public override bool CanRespond(ResponseContext context)
        {
            foreach (KeyValuePair<string,string> kv in context.UserNameCache)
            {
                if (!userNameCache.ContainsKey(kv.Key))
                {
                    userNameCache.Add(kv.Key, kv.Value);
                }
            }
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
