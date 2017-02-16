using MargieBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareBot
{
    public class UsernameCacheResponder : ISBResponder
    {
        public IReadOnlyDictionary<string, string> userNameCache = new Dictionary<string, string>();
        public bool CanRespond(ResponseContext context)
        {
            userNameCache = context.UserNameCache;
            return false;
        }
        public bool CanReact(ResponseContext context)
        {
            return false;
        }
        public BotReaction GetReaction(ResponseContext context) { return new BotReaction(); }

        public BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage { Text = String.Empty };
        }

        public string getUsage()
        {
            return null;
        }
        public string getDescription()
        {
            return null;
        }
        public override string ToString()
        {
            return "Username Cache";
        }
    }
}
