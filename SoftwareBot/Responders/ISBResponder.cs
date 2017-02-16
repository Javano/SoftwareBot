using MargieBot;

namespace SoftwareBot
{
    public interface ISBResponder : IResponder
    {
        string getUsage();
        string getDescription();

    }
}
