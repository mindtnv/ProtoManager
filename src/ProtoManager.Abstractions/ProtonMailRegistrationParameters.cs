namespace ProtoManager.Abstractions;

public class ProtonMailRegistrationOptions
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool Headless { get; set; }
}