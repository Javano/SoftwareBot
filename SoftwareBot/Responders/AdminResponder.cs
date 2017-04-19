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
            BotReaction reaction = new BotReaction { Name = "white_check_mark", Timestamp = context.Message.Timestamp, ChatHub= context.Message.ChatHub };
            string messageStr = context.Message.Text.ToLower();
            if (messageStr.Contains("/delete"))
            {
                bot.DeleteLast(context.Message.ChatHub);
            }
            else if (messageStr.Contains("/kill"))
            {
                ReactAndKill(reaction);
            }
                return reaction;
        }
        private async void ReactAndKill(BotReaction reaction)
        {
            await bot.React(reaction);
            bot.Disconnect();
            Environment.Exit(123);
        }

        public override string ToString()
        {
            return "Administration Interface";
        }


    }
}
