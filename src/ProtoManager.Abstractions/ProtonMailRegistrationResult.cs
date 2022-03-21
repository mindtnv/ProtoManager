namespace ProtoManager.Abstractions;

public class ProtonMailRegistrationResult
{
    public bool IsOk { get; set; }
    public ProtonMailAccount? Account { get; set; }
}