using System.Collections.Generic;

namespace MargieBot
{
    public class BotReaction
    {
        public SlackChatHub ChatHub { get; set; }
        public string Name { get; set; }
        public string Timestamp { get; set; }
        public string File { get; set; }
        public string File_Comment { get; set; }
        public IList<KeyValuePair<string, string>> Custom_Attibs { get; set; }

        public BotReaction()
        {
            Custom_Attibs = new List<KeyValuePair<string, string>>();
        }
    }
}