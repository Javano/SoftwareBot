
using MargieBot;
using System;
using System.Collections.Generic;

namespace SoftwareBot
{
    public class ReactionResponder : ISBResponder
    {
        Dictionary<string,string> reactDict = new Dictionary<string, string>();
        string reactionName = String.Empty;

        public ReactionResponder()
        {
            reactDict.Add("bork", "gabe");
            reactDict.Add("gabe", "gabe");
            reactDict.Add("doggo", "gabe");
            reactDict.Add("pupper", "gabe");
            reactDict.Add("bug", "feature");
            reactDict.Add("doge", "doge");
            reactDict.Add("roll", "rolling");
            reactDict.Add("easy", "easy_button");
        }

        public bool CanRespond(ResponseContext context)
        {
            return false;
        }

        public bool CanReact(ResponseContext context)
        {
            foreach(string key in reactDict.Keys)
            {
                if (context.Message.Text.ToLower().Contains(key.ToLower()))
                {
                    reactionName = reactDict[key];
                    break;
                }
            }
            return (reactionName!= String.Empty)
                && !context.BotHasReacted;
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage { };
        }   

        public BotReaction GetReaction(ResponseContext context)
        {
            BotReaction react = new BotReaction();
            dynamic rData = Newtonsoft.Json.JsonConvert.DeserializeObject(context.Message.RawData);
            string ts = rData["ts"];

            if (ts != String.Empty)
            {
                react.Timestamp = ts;
                react.Name = reactionName;
                react.ChatHub = context.Message.ChatHub;
            }
            reactionName = String.Empty;
            return react;
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
            return "Reaction Responder";
        }
    }
}
