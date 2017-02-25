using MargieBot;

namespace SoftwareBot
{
    public interface ISBResponder : IResponder
    {
        string GetUsage();
        string GetDescription();

    }
}
