using MargieBot;
using System.Text;

namespace SoftwareBot
{
    public class AdminResponder : IResponder
    {
        SoftwareBot bot;
            public AdminResponder(SoftwareBot bot)
        {
            this.bot = bot;
        }
        public bool CanRespond(ResponseContext context)
        {
            string messageLwr = context.Message.Text.ToLower();

            return (context.Message.User.ID.Equals(Properties.Settings.Default.ADMIN_ID) && context.Message.Text.StartsWith("command: ") &&  !context.BotHasResponded);
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var builder = new StringBuilder();
            string commandStr = context.Message.Text.Substring(10);
            bot.DoCommand(commandStr);
            builder.Append("Command Processed: \"" + commandStr + "\"");

            return new BotMessage { Text = builder.ToString() };
        }

        public string getUsage()
        {
            return "<NOT YET DEFINED>";
        }
        public string getDescription()
        {
            return "<NOT YET DEFINED>";
        }

        public override string ToString()
        {
            return "Administration Interface";
        }

    }
}
