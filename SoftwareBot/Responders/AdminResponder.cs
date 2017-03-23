using MargieBot;
using System.Text;
using SlackAPI;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace SoftwareBot
{
    public class AdminResponder : SBResponder
    {
        SoftwareBot bot;
            public AdminResponder(SoftwareBot bot)
        {
            this.bot = bot;
        }
        public override bool CanReact(ResponseContext context)
        {
            string messageLwr = context.Message.Text.ToLower();

            return (context.Message.MentionsBot && context.Message.Text.Contains("/") &&  !context.BotHasReacted);
        }

        public override BotReaction GetReaction(ResponseContext context)
        {
            string messageStr = context.Message.Text.ToLower();
            if (messageStr.Contains("/delete"))
            {
                bot.DeleteLast(context.Message.ChatHub);
            }
            else if (messageStr.Contains("/kill"))
            {
                bot.Disconnect();
                Environment.Exit(123);
            }
                return new BotReaction { Name = "white_check_mark", Timestamp=context.Message.Timestamp };
        }

        public override string ToString()
        {
            return "Administration Interface";
        }


    }
}
