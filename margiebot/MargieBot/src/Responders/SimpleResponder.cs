using System;
using System.Collections.Generic;

namespace MargieBot
{
    public class SimpleResponder : IResponder
    {
        public Func<ResponseContext, bool> CanRespondFunction { get; set; }
        public List<Func<ResponseContext, BotMessage>> GetResponseFunctions { get; set; }
        public Func<ResponseContext, bool> CanReactFunction { get; set; }
        public List<Func<ResponseContext, BotReaction>> GetReactionFunctions { get; set; }

        public SimpleResponder()
        {
            GetResponseFunctions = new List<Func<ResponseContext, BotMessage>>();
        }

        public bool CanRespond(ResponseContext context)
        {
            return CanRespondFunction(context);
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            if (GetResponseFunctions.Count == 0) {
                throw new InvalidOperationException("Attempted to get a response for \"" + context.Message.Text + "\", but no valid responses have been registered.");
            }

            return GetResponseFunctions[new Random().Next(GetResponseFunctions.Count - 1)](context);
        }



        public bool CanReact(ResponseContext context)
        {
            return CanReactFunction(context);
        }

        public BotReaction GetReaction(ResponseContext context)
        {
            if (GetResponseFunctions.Count == 0)
            {
                throw new InvalidOperationException("Attempted to get a reaction for \"" + context.Message.Text + "\", but no valid reactions have been registered.");
            }

            return GetReactionFunctions[new Random().Next(GetReactionFunctions.Count - 1)](context);
        }
        #region Utility
        public static SimpleResponder Create(Func<ResponseContext, bool> canRespond, Func<ResponseContext, string> getResponse, Func<ResponseContext, bool> canReact, Func<ResponseContext, string> getReaction)
        {
            return new SimpleResponder()
            {
                CanRespondFunction = canRespond,
                GetResponseFunctions = new List<Func<ResponseContext, BotMessage>>()
                {
                    (ResponseContext context) =>
                    {
                        return new BotMessage() { Text = getResponse(context) };
                    }
                },

                CanReactFunction = canReact,
                GetReactionFunctions = new List<Func<ResponseContext, BotReaction>>()
                {
                    (ResponseContext context) =>
                    {
                        return new BotReaction() { Name = getReaction(context) };
                    }
                }
            };
        }
        #endregion
    }
}