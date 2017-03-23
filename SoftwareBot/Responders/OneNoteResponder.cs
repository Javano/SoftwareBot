using MargieBot;
using System.Text.RegularExpressions;

namespace SoftwareBot
{
    public class OneNoteResponder : SBResponder
    {
        private static Regex ONENOTE_MASK = new Regex(@"onenote\:\<(?<OneNoteLink>http.+)\>");
        private static string USAGE = "Automatically detect keywords in message";
        private static string CLASSTOSTRING = "OneNote linker responder";
        private static string DESCRIPTION = "Post a link to directly open the OneNote page.";
        public override bool CanRespond(ResponseContext context)
        {
            
            return ONENOTE_MASK.IsMatch(context.Message.Text);
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            MatchCollection matches = ONENOTE_MASK.Matches(context.Message.Text);
            if (matches.Count > 0)
            {
                BotMessage message = new BotMessage();
                int i = 1;                
                foreach (Match match in matches)
                {
                    SlackAttachment attachment = new SlackAttachment()
                    {
                        Title = $"Open in OneNote ({i}/{matches.Count})"
                    };
                    string onenoteLink = match.Groups["OneNoteLink"].Value;
                    onenoteLink.Replace("&", "&amp;");
                    onenoteLink = "onenote://" + onenoteLink;
                    attachment.TitleLink = onenoteLink;
                    message.Attachments.Add(attachment);
                    i++;
                }

                // Manually fetch message JSON
                dynamic rData = Newtonsoft.Json.JsonConvert.DeserializeObject(context.Message.RawData);


                // Use the original's timestamp as the reply's default threading timestamp

                /** Disabled because Onenote links shouldn't be threaded unless triggered in a thread.
                if (rData["ts"] != null)
                {
                    string ts = rData["ts"];
                    message.Thread_TS = ts;
                } **/

                // If the original message is already in a thread, just use that thread timestamp instead.
                if (rData["thread_ts"] != null)
                {
                    string thread_ts = rData["thread_ts"];
                    message.Thread_TS = thread_ts;
                }
                return message;
            }
            else
            {
                return new BotMessage { Text = "Hmm I should reply but have not found one note link :|" };
            }
        }

        public override string GetUsage() => USAGE;
        public override string GetDescription() => DESCRIPTION;
        public override string ToString() => CLASSTOSTRING;
    }
}
