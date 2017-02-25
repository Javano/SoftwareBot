using MargieBot;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareBot
{
    public class HelpResponder : SBResponder
    {
        private List<IResponder> responders;

        public HelpResponder(List<IResponder> responders)
        {
            this.responders = responders;
        }

        public override bool CanRespond(ResponseContext context)
        {
            return !context.BotHasResponded
                && context.Message.MentionsBot
                  && (context.Message.Text.ToLower().Contains("help") || context.Message.Text.ToLower().Contains("commands"));
        }



            public override BotMessage GetResponse(ResponseContext context)
        {
            var builder = new StringBuilder();
            builder.Append("Available Commands:\n");
            foreach (IResponder r in responders)
            {
                try { 
                    ISBResponder r2 = (ISBResponder)r;
                    if (r2.GetUsage() != null && r2.GetDescription() != null)
                    {
                        builder.Append("`").Append(r2.GetUsage()).Append("`\n```").Append(r2.GetDescription()).Append("```\n");
                    }
                } catch (Exception) 
                {

                } 
            }
            return new BotMessage { Text = builder.ToString() };
        }

        public override string GetUsage()
        {
            return "@SoftwareBot help";
        }
        public override string GetDescription()
        {
            return "Responds with a list of available commands.";
        }

        public override string ToString()
        {
            return "Help Command";
        }
    }
}
