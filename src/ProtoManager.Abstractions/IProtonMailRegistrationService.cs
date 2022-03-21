namespace ProtoManager.Abstractions;

public interface IProtonMailRegistrationService
{
    Task<ProtonMailRegistrationResult> RegisterAsync(ProtonMailRegistrationOptions options,
        CancellationToken cancellationToken);
}