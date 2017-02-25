using MargieBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareBot
{
    public class SBResponder : ISBResponder
    {
        public virtual bool CanReact(ResponseContext context)
        {
            return false;
        }
        public virtual BotReaction GetReaction(ResponseContext context) {
            return new BotReaction();
        }
        public virtual bool CanRespond(ResponseContext context)
        {
            return false;
        }
        public virtual BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage();
        }

        public virtual string GetUsage()
        {
            return null;
        }

        public virtual string GetDescription()
        {
            return null;
        }
    }
}
