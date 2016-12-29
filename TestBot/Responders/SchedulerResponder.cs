using MargieBot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SoftwareBot
{
    public class SchedulerResponder : ISBResponder
    {
        DateTime scheduleDate = DateTime.MinValue;
        string content = "";
        public IReadOnlyDictionary<string, string> userNameCache = new Dictionary<string, string>();
        private BindingList<ScheduledItem> schedule;
        private int repeatMode = ScheduledItem.REPEAT_NONE;
        public SchedulerResponder(BindingList<ScheduledItem> schedule)
        {
            this.schedule = schedule;
        }


        public bool CanRespond(ResponseContext context)
        {
            userNameCache = context.UserNameCache;

            scheduleDate = DateTime.MinValue;

            var regex = new Regex(@"schedule\s*(\d\d\W\d\d\W\d\d\d\d\s*\d\d\W\d\d)\s*(hourly|daily|weekly|monthly|yearly)?\s*(.*)");

            Match match = regex.Match(context.Message.Text);
            if (match.Success)
            {
                DateTime.TryParse(match.Groups[1].Value, out scheduleDate);
                content = match.Groups[match.Groups.Count - 1].Value;

                if (match.Groups.Count > 3)
                {
                    switch (match.Groups[2].Value.ToLower())
                    {
                        case ("hourly"):
                            repeatMode = ScheduledItem.REPEAT_HOURLY;
                            break;
                        case ("daily"):
                            repeatMode = ScheduledItem.REPEAT_DAILY;
                            break;
                        case ("weekly"):
                            repeatMode = ScheduledItem.REPEAT_WEEKLY;
                            break;
                        case ("monthly"):
                            repeatMode = ScheduledItem.REPEAT_MONTHLY;
                            break;
                        case ("yearly"):
                            repeatMode = ScheduledItem.REPEAT_YEARLY;
                            break;
                        default:
                            repeatMode = ScheduledItem.REPEAT_NONE;
                            break;
                    }
                }
            }


            return !context.BotHasResponded
                    && context.Message.MentionsBot
                      && context.Message.Text.Contains("schedule");
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var builder = new StringBuilder();
            if (scheduleDate != DateTime.MinValue)
            {
                ScheduledItem newItem = new ScheduledItem(scheduleDate, content, context.Message.ChatHub, repeatMode);
                schedule.Add(newItem);
                builder.Append("_New item successfully scheduled for:_ ").Append("*" + scheduleDate.ToLongDateString() + "* _at_ *" + scheduleDate.ToShortTimeString() + "*");
                switch (repeatMode)
                {
                    case (ScheduledItem.REPEAT_HOURLY):
                        builder.Append(" -- _Repeating hourly._");
                        break;
                    case (ScheduledItem.REPEAT_DAILY):
                        builder.Append(" -- _Repeating daily._");
                        break;
                    case (ScheduledItem.REPEAT_WEEKLY):
                        builder.Append(" -- _Repeating weekly._");
                        break;
                    case (ScheduledItem.REPEAT_MONTHLY):
                        builder.Append(" -- _Repeating monthly._");
                        break;
                    case (ScheduledItem.REPEAT_YEARLY):
                        builder.Append(" -- _Repeating yearly._");
                        break;
                    default:
                        builder.Append(" -- _No repeat._");
                        break;
                }
                

            }
            else
            {

                builder.Append("Error scheduling item. Invalid date/time!\nFor help use the *Help* command!");
            }
            return new BotMessage { Text = builder.ToString()};
        }

        public string getUsage()
        {
            return "@SoftwareBot schedule {DATE} {TIME} {?REPEAT?} {MESSAGE}";
        }
        public string getDescription()
        {
            return "Schedules the specified message to be sent at the given date and time.\n\t-DATE/TIME: MM/DD/YYYY HH:MM. \n\t-REPEAT: Hourly, Daily, Weekly, Monthly, Yearly.";
        }
        public override string ToString()
        {
            return "Reminder Scheduler";
        }
    }
}
