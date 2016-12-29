using MargieBot;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareBot
{
    public class HelpResponder : ISBResponder
    {
        private List<IResponder> responders;

        public HelpResponder(List<IResponder> responders)
        {
            this.responders = responders;
        }

        public bool CanRespond(ResponseContext context)
        {
            return !context.BotHasResponded
                && context.Message.MentionsBot
                  && (context.Message.Text.ToLower().Contains("help") || context.Message.Text.ToLower().Contains("commands"));
        }


        public BotMessage GetResponse(ResponseContext context)
        {
            var builder = new StringBuilder();
            builder.Append("Available Commands:\n");
            foreach (IResponder r in responders)
            {
                try { 
                    ISBResponder r2 = (ISBResponder)r;
                    builder.Append("`").Append(r2.getUsage()).Append("`\n```").Append(r2.getDescription()).Append("```\n");
                } catch (Exception) 
                {

                } 
            }
            // builder.Append("Hello ").Append(context.Message.User.FormattedUserID);
            return new BotMessage { Text = builder.ToString() };
        }

        public string getUsage()
        {
            return "@SoftwareBot help";
        }
        public string getDescription()
        {
            return "Responds with a list of available commands.";
        }

        public override string ToString()
        {
            return "Help Command";
        }
    }
}
