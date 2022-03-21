using PuppeteerSharp;

namespace ProtoManager;

public interface IBrowserProvider : IAsyncDisposable
{
    Browser GetBrowser();
}