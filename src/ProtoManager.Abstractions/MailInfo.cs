namespace ProtoManager.Abstractions;

public class MailInfo
{
    public string Id { get; set; }
    public SenderInfo Sender { get; set; }
    public string Subject { get; set; }
}