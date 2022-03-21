using PuppeteerSharp;

namespace ProtoManager;

public class BrowserProvider : IBrowserProvider
{
    private readonly Browser _browser;

    public BrowserProvider(Browser browser)
    {
        _browser = browser;
    }

    public Browser GetBrowser() => _browser;
    public ValueTask DisposeAsync() => _browser.DisposeAsync();
}