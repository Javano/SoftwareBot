using MargieBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareBot
{
    public class TimeResponder : SBResponder
    {
        public IReadOnlyDictionary<string, string> userNameCache = new Dictionary<string, string>();
        public override bool CanRespond(ResponseContext context)
        {
            userNameCache = context.UserNameCache;
            return !context.BotHasResponded
                && context.Message.MentionsBot
                  && context.Message.Text.ToLower().Contains("time");
        }


        public override BotMessage GetResponse(ResponseContext context)
        {
            var builder = new StringBuilder();
            // builder.Append("Hello ").Append(context.Message.User.FormattedUserID);
            builder.Append(context.Message.User.FormattedUserID).Append(", the time is: ").Append(DateTime.Now.ToLongTimeString());
            return new BotMessage { Text = builder.ToString() };
        }

        public override string GetUsage()
        {
            return "@SoftwareBot time";
        }
        public override string GetDescription()
        {
            return "Responds with the current time.";
        }
        public override string ToString()
        {
            return "Time Notifier";
        }
    }
}
