namespace ProtoManager.Abstractions;

public interface IProtonMailManager : IAsyncDisposable
{
    public Task<IEnumerable<MailInfo>> GetMailInfosAsync(int page);
    public Task<Mail> GetMailAsync(string id);
}