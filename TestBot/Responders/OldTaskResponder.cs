using MargieBot;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SoftwareBot
{
    public class OldTaskResponder : ISBResponder
    {
        List<string> taskNums = new List<string>();
        public bool CanRespond(ResponseContext context)
        {
            taskNums = new List<string>();
            string messageLwr = context.Message.Text.ToLower();
            Regex reg = new Regex(@"\@\d{4}");
            Match m = reg.Match(messageLwr);

            foreach (Match match in reg.Matches(messageLwr))
            {
                taskNums.Add(match.Value.Substring(1));
            }

            return (taskNums.Count > 0 && !context.BotHasResponded);
        }


        public BotMessage GetResponse(ResponseContext context)
        {
            var builder = new StringBuilder();

            foreach (string taskNum in taskNums)
            {
                builder.Append(@"http://tfs:8080/tfs/MainProjects/IAFCore/_workitems#_a=edit&id=").Append(taskNum).Append("\n");
            }
            return new BotMessage { Text = builder.ToString() };
        }

        public string getUsage()
        {
            return "@{TASK_NUM}";
        }
        public string getDescription()
        {
            return "[OBSOLETE PLEASE USE NEW TASKRESPONDER]Replies with a URL linking directly to the specified TFS task.";
        }


        public override string ToString()
        {
            return "TFS Task Linker";
        }
    }
}
