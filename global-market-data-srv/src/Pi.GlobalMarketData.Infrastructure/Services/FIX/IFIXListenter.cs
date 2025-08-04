using QuickFix;

public interface IFIXListener : IApplication
{
    void SendMessage(Message message);
}