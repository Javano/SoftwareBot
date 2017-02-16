using MargieBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareBot
{
    public class TimeResponder : ISBResponder
    {
        public IReadOnlyDictionary<string, string> userNameCache = new Dictionary<string, string>();
        public bool CanRespond(ResponseContext context)
        {
            userNameCache = context.UserNameCache;
            return !context.BotHasResponded
                && context.Message.MentionsBot
                  && context.Message.Text.ToLower().Contains("time");
        }
        public bool CanReact(ResponseContext context)
        {
            return false;
        }
        public BotReaction GetReaction(ResponseContext context) { return new BotReaction(); }

        public BotMessage GetResponse(ResponseContext context)
        {
            var builder = new StringBuilder();
            // builder.Append("Hello ").Append(context.Message.User.FormattedUserID);
            builder.Append(context.Message.User.FormattedUserID).Append(", the time is: ").Append(DateTime.Now.ToLongTimeString());
            return new BotMessage { Text = builder.ToString() };
        }

        public string getUsage()
        {
            return "@SoftwareBot time";
        }
        public string getDescription()
        {
            return "Responds with the current time.";
        }
        public override string ToString()
        {
            return "Time Notifier";
        }
    }
}
