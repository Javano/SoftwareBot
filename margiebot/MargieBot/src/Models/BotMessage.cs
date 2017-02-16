using System.Collections.Generic;

namespace MargieBot
{
    public class BotMessage
    {
        public IList<SlackAttachment> Attachments { get; set; }
        public SlackChatHub ChatHub { get; set; }
        public string Text { get; set; }

        /** CUSTOM MEMBERS ****************************************/
        public string Thread_TS { get; set; }
        public IList<KeyValuePair<string, string>> Custom_Attibs { get; set; }
        /** END CUSTOM MEMBERS ************************************/

        public BotMessage()
        {
            Attachments = new List<SlackAttachment>();
            Custom_Attibs = new List<KeyValuePair<string, string>>();
        }
    }
}