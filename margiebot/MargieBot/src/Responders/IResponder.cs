namespace MargieBot
{
    public interface IResponder
    {
        bool CanRespond(ResponseContext context);
        bool CanReact(ResponseContext context);
        BotMessage GetResponse(ResponseContext context);
        BotReaction GetReaction(ResponseContext context);
    }
}