using MargieBot;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SoftwareBot
{
    [Serializable]
    public class ScheduledItem : ISerializable
    {
        public const int REPEAT_NONE = 0;
        public const int REPEAT_HOURLY = 1;
        public const int REPEAT_DAILY = 2;
        public const int REPEAT_WEEKLY = 3;
        public const int REPEAT_MONTHLY = 4;
        public const int REPEAT_YEARLY = 5;

        private string content = String.Empty;
        private DateTime date;
        private SlackChatHub chatHub;
        private int repeatMode;
        public ScheduledItem(DateTime date, string content, SlackChatHub chatHub, int repeatMode = 0)
        {
            this.content = content;
            this.date = date;
            this.chatHub = chatHub;
            this.repeatMode = repeatMode;
        }

        protected ScheduledItem(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            Enum.TryParse<SlackChatHubType>(info.GetString("ChatHub_Type"), out SlackChatHubType hubType);
            content = info.GetString("Content");
            chatHub =  new SlackChatHub() { ID = info.GetString("ChatHub_ID"), Name = info.GetString("ChatHub_Name"), Type = hubType };
            date = info.GetDateTime("Date");
            repeatMode = info.GetInt32("RepeatMode");
        }

        public void Reschedule()
        {
            switch (repeatMode)
            {
                case (REPEAT_HOURLY):
                    date = date.AddHours(1);
                    break;
                case (REPEAT_DAILY):
                    date = date.AddDays(1);
                    break;
                case (REPEAT_WEEKLY):
                    date = date.AddDays(7);
                    break;
                case (REPEAT_MONTHLY):
                    date = date.AddMonths(1);
                    break;
                case (REPEAT_YEARLY):
                    date = date.AddYears(1);
                    break;
                default:
                    Console.WriteLine("ERROR: Could not reschedule event with no reschedule mode!");
                    break;
            }
        }
        
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        public int RepeatMode
        {
            get
            {
                return repeatMode;
            }
            set
            {
                repeatMode = value;
            }
        }

        public bool IsResechedulable
        {
            get
            {
               return repeatMode != REPEAT_NONE;
            }
        }
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
            }
        }
        public SlackChatHub ChatHub
        {
            get
            {
                return chatHub;
            }
            set
            {
                chatHub = value;
            }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("Date", date);
            info.AddValue("ChatHub_ID", chatHub.ID);
            info.AddValue("ChatHub_Name", chatHub.Name);
            info.AddValue("ChatHub_Type", Enum.GetName(chatHub.Type.GetType(), chatHub.Type));
            info.AddValue("Content", content);
            info.AddValue("RepeatMode", repeatMode);

        } 
    }
}
