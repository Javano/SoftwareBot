namespace MargieBot
{
    public class SlackMessage
    {
        public SlackChatHub ChatHub { get; set; }
        public bool MentionsBot { get; set; }
        public string RawData { get; set; }
        public string Text { get; set; }
        public SlackUser User { get; set; }

        /** CUSTOM MEMBERS ****************************************/
        public string Timestamp { get; set; }
        public string Thread_TS { get; set; }
        /** END CUSTOM MEMBERS ************************************/
    }
}