using MargieBot;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SoftwareBot
{
    public class ChatResponder : SBResponder
    {


        private string API_URI_SUFFIX;
        private string API_URI = "http://www.cleverbot.com/";
        public ChatResponder(string apiKey)
        {
            API_URI_SUFFIX = "getreply?key=" + apiKey;
        }

        public override bool CanRespond(ResponseContext context)
        {
            return !context.BotHasResponded
                  && context.Message.MentionsBot;
        }


        public override BotMessage GetResponse(ResponseContext context)
        {

            var builder = new StringBuilder();
            try
            {

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


                using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
                {
                    client.BaseAddress = new Uri(API_URI);
                    HttpResponseMessage response = client.GetAsync(API_URI_SUFFIX + "&input=" + thought).Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic respObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                    thought = respObj["output"];
                }

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

        public override BotReaction GetReaction(ResponseContext context)
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
        public override string GetUsage()
        {
            return "@SoftwareBot {MESSAGE}";
        }

        public override string GetDescription()
        {
            return "Prompts a reply from SoftwareBot.";
        }

        public override string ToString()
        {
            return "Smart Responses";
        }
    }
}
