using ChatterBotAPI;
using MargieBot;
using System.Text;

namespace SoftwareBot
{
    public class ChatResponder : ISBResponder
    {

        ChatterBotFactory factory = new ChatterBotFactory();
        ChatterBotSession bot1session = null;
        ChatterBot bot1 = null;

        public ChatResponder()
        {
            
        }

        public bool CanRespond(ResponseContext context)
        {
            return !context.BotHasResponded
                  && context.Message.MentionsBot;
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            if(bot1 == null)
            {
                bot1 = factory.Create(ChatterBotType.CLEVERBOT);
            }

            if(bot1session == null)
            {
                bot1session = bot1.CreateSession();
            }

           string removeString = @"<@U18H7MEPL>";
           int index = context.Message.Text.IndexOf(removeString);
            string thought = context.Message.Text;
            if (index == 0)
            {
                thought = thought.Remove(index, removeString.Length);
                
            }

            thought = thought.Replace(removeString, "CleverBot");
            thought = thought.Replace("SoftwareBot", "CleverBot");
            thought = thought.Replace("SoftwareBot", "CleverBot");
            thought = thought.Replace("softwarebot", "CleverBot");
            thought = thought.Replace("Softwarebot", "CleverBot");
            thought = bot1session.Think(thought);
            thought = thought.Replace("CleverBot", "SoftwareBot");
            thought = thought.Replace("cleverbot", "SoftwareBot");
            thought = thought.Replace("Clever Bot", "SoftwareBot");
            thought = thought.Replace("clever bot", "SoftwareBot");
            thought = thought.Replace("clever bot", "SoftwareBot");
            var builder = new StringBuilder();
            // builder.Append("Hello ").Append(context.Message.User.FormattedUserID);
            builder.Append(thought); 

            return new BotMessage { Text = builder.ToString() };
        }

        public string getUsage()
        {
            return "@SoftwareBot {MESSAGE}";
        }

        public string getDescription()
        {
            return "Prompts a reply from SoftwareBot.";
        }


        public override string ToString()
        {
            return "Smart Responses";
        }
    }
}
