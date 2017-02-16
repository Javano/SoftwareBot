using ChatterBotAPI;
using MargieBot;
using System;
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

        public bool CanReact(ResponseContext context)
        {
            return false;
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            
            var builder = new StringBuilder();
            try
            {
                if (bot1 == null)
                {
                    bot1 = factory.Create(ChatterBotType.CLEVERBOT);
                }

                if (bot1session == null)
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

                // builder.Append("Hello ").Append(context.Message.User.FormattedUserID);
                builder.Append(thought);
            } catch(Exception ex)
            {
                builder.Append("`ERROR: " + ex.Message + "`");
            }
            BotMessage reply = new BotMessage { Text = builder.ToString() };

            dynamic rData = Newtonsoft.Json.JsonConvert.DeserializeObject(context.Message.RawData);
            string thread = rData["thread_ts"];
            string ts = rData["ts"];
            if (ts != null && ts != String.Empty)
            {
                reply.Thread_TS = ts;
            }
            if (thread != null && thread != String.Empty)
            {
               reply.Thread_TS = thread;
            }

            return reply;
        }   

        public BotReaction GetReaction(ResponseContext context)
        {
            BotReaction react = new BotReaction();
            dynamic rData = Newtonsoft.Json.JsonConvert.DeserializeObject(context.Message.RawData);
            string ts = rData["ts"];

            if (ts != String.Empty)
            {
                react.Timestamp = ts;
                react.Name = "gabe";
                react.ChatHub = context.Message.ChatHub;
            }
            return react;
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
