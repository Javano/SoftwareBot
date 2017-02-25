
using MargieBot;
using System;
using System.Collections.Generic;

namespace SoftwareBot
{
    public class ReactionResponder : SBResponder
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

 

        public override bool CanReact(ResponseContext context)
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


        public override BotReaction GetReaction(ResponseContext context)
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

        public override string ToString()
        {
            return "Reaction Responder";
        }
    }
}
