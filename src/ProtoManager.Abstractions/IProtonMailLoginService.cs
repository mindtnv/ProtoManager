namespace ProtoManager.Abstractions;

public interface IProtonMailLoginService
{
    Task<IProtonMailManager> LoginAsync(ProtonMailLoginOptions options, CancellationToken cancellationToken);
}